using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Installation
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ITechnicianAssignmentRepository _assignmentRepository;
        private readonly IElevatorRepository _elevatorRepository; // Need to fetch Elevator for finish date calculation
        private readonly IInstallationStageRepository _stageRepository; // For stats calculation
        private readonly IStageTechnicianRepository _stageTechnicianRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TechnicianService(
            ITechnicianRepository technicianRepository,
            ITechnicianAssignmentRepository assignmentRepository,
            IElevatorRepository elevatorRepository, 
            IInstallationStageRepository stageRepository,
            IStageTechnicianRepository stageTechnicianRepository,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            ApplicationDbContext context)
        {
            _technicianRepository = technicianRepository;
            _assignmentRepository = assignmentRepository;
            _elevatorRepository = elevatorRepository;
            _stageRepository = stageRepository;
            _stageTechnicianRepository = stageTechnicianRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        public async Task<Technician> AddTechnicianAsync(Technician technician)
        {
            _technicianRepository.Add(technician);
            await _unitOfWork.Complete();
            return technician;
        }

        public async Task<Technician> UpdateTechnicianAsync(Technician technician)
        {
            _technicianRepository.Update(technician);
            await _unitOfWork.Complete();
            return technician;
        }

        public async Task DisableTechnicianAsync(Guid id, bool disable)
        {
            var tech = await _technicianRepository.GetByIdAsync(id);
            if (tech == null) throw new Exception("Technician not found");

            tech.IsDisabled = disable;
            _technicianRepository.Update(tech);

            // If disabling a leader, also disable all their subordinates
            if (disable)
            {
                var subordinates = await _technicianRepository.GetSubordinatesByLeaderIdAsync(id);
                foreach (var subordinate in subordinates)
                {
                    subordinate.IsDisabled = true;
                    _technicianRepository.Update(subordinate);
                }
            }

            await _unitOfWork.Complete();
        }

        public async Task DeleteTechnicianAsync(Guid id)
        {
            var tech = await _technicianRepository.GetTechnicianWithAssignmentsAsync(id);
            if (tech == null) throw new Exception("Technician not found");

            // 1. Delete all StageTechnician assignments (has DeleteBehavior.Restrict)
            var stageTechnicians = await _stageTechnicianRepository.GetByTechnicianIdAsync(id);
            foreach (var stageTech in stageTechnicians)
            {
                _stageTechnicianRepository.Delete(stageTech);
            }

            // 2. Delete all TechnicianAssignment records (has DeleteBehavior.Restrict)
            if (tech.Assignments != null && tech.Assignments.Any())
            {
                foreach (var assignment in tech.Assignments.ToList())
                {
                    _assignmentRepository.Delete(assignment);
                }
            }

            // 3. Set TechnicianId to null in MaintenanceVisit records
            var maintenanceVisits = await _context.MaintenanceVisits
                .Where(v => v.TechnicianId == id)
                .ToListAsync();
            foreach (var visit in maintenanceVisits)
            {
                visit.TechnicianId = null;
                _context.MaintenanceVisits.Update(visit);
            }

            // 4. Set TechnicianId to null in MaintenanceContract records
            var maintenanceContracts = await _context.MaintenanceContracts
                .Where(c => c.TechnicianId == id)
                .ToListAsync();
            foreach (var contract in maintenanceContracts)
            {
                contract.TechnicianId = null;
                _context.MaintenanceContracts.Update(contract);
            }

            // 5. Set AssignedTechnicianId to null in EmergencyTicket records
            var emergencyTickets = await _context.EmergencyTickets
                .Where(t => t.AssignedTechnicianId == id)
                .ToListAsync();
            foreach (var ticket in emergencyTickets)
            {
                ticket.AssignedTechnicianId = null;
                _context.EmergencyTickets.Update(ticket);
            }

            // 6. Set AssignedTechnicianId to null in FaultTicket records
            var faultTickets = await _context.FaultTickets
                .Where(t => t.AssignedTechnicianId == id)
                .ToListAsync();
            foreach (var ticket in faultTickets)
            {
                ticket.AssignedTechnicianId = null;
                _context.FaultTickets.Update(ticket);
            }

            // 7. If technician is a leader, remove leader relationship from subordinates
            var subordinates = await _technicianRepository.GetSubordinatesByLeaderIdAsync(id);
            foreach (var subordinate in subordinates)
            {
                subordinate.LeaderId = null;
                _technicianRepository.Update(subordinate);
            }

            // 8. Delete linked AppUser if exists
            if (tech.UserId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(tech.UserId.Value.ToString());
                if (user != null)
                {
                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to delete user account: {errors}");
                    }
                }
            }

            // 9. Delete the technician
            _technicianRepository.Delete(tech);
            await _unitOfWork.Complete();
        }

        public async Task AssignTechnicianToElevatorAsync(Guid technicianId, Guid elevatorId, Guid assignedByUserId)
        {
            var tech = await _technicianRepository.GetByIdAsync(technicianId);
            if (tech == null) throw new Exception("Technician not found");
            if (tech.IsDisabled) throw new Exception("Cannot assign disabled technician");

            // Check if technician's leader is disabled
            if (tech.LeaderId.HasValue)
            {
                var leader = await _technicianRepository.GetByIdAsync(tech.LeaderId.Value);
                if (leader != null && leader.IsDisabled)
                {
                    throw new Exception("Cannot assign technician. The technician's leader is inactive.");
                }
            }

            var elevator = await _elevatorRepository.GetByIdAsync(elevatorId);
            if (elevator == null) throw new Exception("Elevator not found");

            // Check if already assigned
            // Simple check: we rely on repo or collection from tech if loaded. 
            // Better to use specification or direct query if collection large.
            // Assumption: not double assigning same tech to same elevator
            
            var assignment = new TechnicianAssignment
            {
                TechnicianId = technicianId,
                ElevatorId = elevatorId,
                AssignedBy = assignedByUserId,
                AssignedAt = DateTime.UtcNow,
                ExpectedFinishDate = DateTime.UtcNow.AddMonths(1) // Placeholder logic or fetch from Project
            };

            _assignmentRepository.Add(assignment);
            
            // Update Stats
            tech.CurrentActiveElevatorsCount++;
            _technicianRepository.Update(tech);

            await _unitOfWork.Complete();
        }

        public async Task UnassignTechnicianFromElevatorAsync(Guid technicianId, Guid elevatorId)
        {
             // Find assignment
             // Delete assignment
             // Decrement count
             // UnitOfWork.Complete
             // Implementation omitted for brevity in this step, focusing on Add/Assign first.
             // But let's implement basic version:
             var tech = await _technicianRepository.GetTechnicianWithAssignmentsAsync(technicianId);
             if (tech == null) return;

             var assignment = tech.Assignments.FirstOrDefault(a => a.ElevatorId == elevatorId);
             if (assignment != null)
             {
                 _assignmentRepository.Delete(assignment);
                 tech.CurrentActiveElevatorsCount = Math.Max(0, tech.CurrentActiveElevatorsCount - 1);
                 _technicianRepository.Update(tech);
                 await _unitOfWork.Complete();
             }
        }

        public async Task<Technician?> GetTechnicianByUserIdAsync(Guid userId)
        {
            return await _technicianRepository.GetTechnicianByUserIdAsync(userId);
        }

        public async Task<IReadOnlyList<Technician>> GetTechniciansAsync()
        {
            return await _technicianRepository.ListAllAsync();
        }

        public async Task<IReadOnlyList<Technician>> GetAvailableTechniciansAsync()
        {
            return await _technicianRepository.GetAvailableTechniciansAsync();
        }

        public async Task UpdateTechnicianStatsAsync(Guid elevatorId)
        {
            // Called when Elevator moves to Stage 4 Success
            // Find all techs assigned to this elevator
            // Remove assignment (or mark complete?), decrement active, increment total installed.
            
            // Strategy: Keep assignment record but maybe mark it "completed"? 
            // Domain doesn't have "Completed" on Assignment. 
            // We'll just delete assignment (Remove from active list) as per requirement: "Remove elevator from active list"
            // But we might want history. 
            // Re-reading requirements: "Reduce technician’s active elevator count. Remove elevator from active list. Increase TotalElevatorsInstalled."
            
            // If we delete the row, we lose history of "who installed what". 
            // Better to keep row but maybe soft-delete or ignore it in "Active" count?
            // "CurrentActiveElevators" is the list. 
            // If we assume `Assignments` collection IS the active list, then removing means deleting row.
            // If we want history, we need `IsCompleted` on Assignment.
            // The Requirement: "CurrentActiveElevators (List...)"
            
            // DECISION: For now, I will DELETE the assignment to satisfy "Remove from active list" strictly. 
            // To keep history, we should have added `IsCompleted` to `TechnicianAssignment`, but schema is set in this turn.
            // I'll proceed with Unassign logic + Increment Total.
            
            // Actually, I can just not delete and use a filter, but I don't have a status field on assignment.
            // I'll stick to: Unassign (Delete) + Increment Total Count.
             
            // Implementation pending query support to find assignments by ElevatorId.
        }
    }
}
