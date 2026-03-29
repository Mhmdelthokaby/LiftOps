using Collins_BackEnd.Domain.Entities.Installation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Interfaces.Installation
{
    public interface ITechnicianService
    {
        Task<Technician> AddTechnicianAsync(Technician technician);
        Task<Technician> UpdateTechnicianAsync(Technician technician);
        Task DisableTechnicianAsync(Guid id, bool disable);
        Task DeleteTechnicianAsync(Guid id);
        
        Task AssignTechnicianToElevatorAsync(Guid technicianId, Guid elevatorId, Guid assignedByUserId);
        Task UnassignTechnicianFromElevatorAsync(Guid technicianId, Guid elevatorId);
        
        Task<IReadOnlyList<Technician>> GetTechniciansAsync();
        Task<IReadOnlyList<Technician>> GetAvailableTechniciansAsync();
        Task<Technician?> GetTechnicianByUserIdAsync(Guid userId);
        
        // Stats update triggered by Stage completion
        Task UpdateTechnicianStatsAsync(Guid elevatorId);
    }
}
