using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces.Maintenance;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Maintenance
{
    public class MaintenanceRepository : GenericRepository<MaintenanceContract>, IMaintenanceRepository
    {
        public MaintenanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<MaintenanceContract>> GetContractsByCustomerAsync(Guid customerId)
        {
            return await _context.MaintenanceContracts
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Elevators)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MaintenanceContract>> GetAllContractsWithDetailsAsync()
        {
            return await _context.MaintenanceContracts
                .Include(c => c.Customer)
                .Include(c => c.Elevators)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<MaintenanceContract?> GetContractByIdWithDetailsAsync(Guid contractId)
        {
            return await _context.MaintenanceContracts
                .Include(c => c.Customer)
                .Include(c => c.Elevators)
                .FirstOrDefaultAsync(c => c.Id == contractId);
        }

        public async Task<MaintenanceElevator?> GetElevatorByIdAsync(Guid id)
        {
            return await _context.MaintenanceElevators
                .Include(e => e.Contract)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IReadOnlyList<MaintenanceElevator>> GetElevatorsByContractAsync(Guid contractId)
        {
            return await _context.MaintenanceElevators
                .Where(e => e.ContractId == contractId)
                .ToListAsync();
        }

        public async Task UpdateElevatorAsync(MaintenanceElevator elevator)
        {
            _context.MaintenanceElevators.Update(elevator);
             // Note: GenericRepository usually assumes changes are tracked, but Update ensures state is Modified.
             // Usually UoW.Complete calls SaveChanges.
             await Task.CompletedTask; 
        }

        public async Task<MaintenanceVisit?> GetVisitByIdAsync(Guid id)
        {
             return await _context.MaintenanceVisits
                .Include(v => v.MaintenanceElevator)
                .Include(v => v.Technician)
                .Include(v => v.SpareParts).ThenInclude(sp => sp.InventoryItem)
                .Include(v => v.ChecklistItems).ThenInclude(ci => ci.ChecklistItem)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByElevatorAsync(Guid elevatorId)
        {
            return await _context.MaintenanceVisits
                .Where(v => v.MaintenanceElevatorId == elevatorId)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByElevatorAndMonthAsync(Guid elevatorId, int? month = null, int? year = null)
        {
            var query = _context.MaintenanceVisits
                .Include(v => v.MaintenanceElevator)
                .Include(v => v.Technician)
                .Include(v => v.ChecklistItems).ThenInclude(ci => ci.ChecklistItem)
                .Where(v => v.MaintenanceElevatorId == elevatorId);

            if (month.HasValue)
            {
                query = query.Where(v => v.VisitDate.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(v => v.VisitDate.Year == year.Value);
            }

            return await query
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetMonthlyScheduleAsync(int month, int year)
        {
            return await _context.MaintenanceVisits
                .Include(v => v.MaintenanceElevator)
                .ThenInclude(e => e.Contract)
                .ThenInclude(c => c.Customer)
                .Include(v => v.Technician)
                .Where(v => v.VisitDate.Month == month && v.VisitDate.Year == year)
                .OrderBy(v => v.VisitDate)
                .ThenBy(v => v.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByContractAndMonthAsync(Guid contractId, int month, int year)
        {
            return await _context.MaintenanceVisits
                .Include(v => v.MaintenanceElevator)
                .Include(v => v.Technician)
                .Include(v => v.ChecklistItems).ThenInclude(ci => ci.ChecklistItem)
                .Where(v => v.MaintenanceElevator.ContractId == contractId && 
                           v.VisitDate.Month == month && 
                           v.VisitDate.Year == year)
                .OrderBy(v => v.VisitDate)
                .ThenBy(v => v.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByDateAsync(DateTime date)
        {
            return await _context.MaintenanceVisits
                .Include(v => v.MaintenanceElevator)
                    .ThenInclude(e => e.Contract)
                .Include(v => v.Technician)
                .Where(v => v.VisitDate.Date == date.Date)
                .OrderBy(v => v.DisplayOrder)
                .ThenBy(v => v.VisitDate)
                .ToListAsync();
        }

        public async Task<bool> HasCompletedVisitForMonthAsync(Guid elevatorId, int month, int year)
        {
            return await _context.MaintenanceVisits
                .AnyAsync(v => v.MaintenanceElevatorId == elevatorId &&
                              v.VisitDate.Month == month &&
                              v.VisitDate.Year == year &&
                              v.Status == Domain.Entities.Maintenance.VisitStatus.Done);
        }

        public async Task AddVisitAsync(MaintenanceVisit visit)
        {
             await _context.MaintenanceVisits.AddAsync(visit);
        }

        public async Task UpdateVisitAsync(MaintenanceVisit visit)
        {
            _context.MaintenanceVisits.Update(visit);
            await Task.CompletedTask;
        }

        public async Task<IReadOnlyList<MaintenanceElevator>> GetAllElevatorsWithDetailsAsync()
        {
            return await _context.MaintenanceElevators
                .Include(e => e.Contract)
                    .ThenInclude(c => c.Customer)
                .OrderBy(e => e.Contract.Customer.Name)
                .ThenBy(e => e.CreatedAt)
                .ToListAsync();
        }
    }
}
