using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface ITechnicianRepository : IGenericRepository<Technician>
    {
        Task<IReadOnlyList<Technician>> GetAvailableTechniciansAsync(int maxActiveProjects = 5);
        Task<Technician?> GetTechnicianWithAssignmentsAsync(Guid id);
        Task<IReadOnlyList<Technician>> GetSubordinatesByLeaderIdAsync(Guid leaderId);
        Task<Technician?> GetTechnicianByUserIdAsync(Guid userId);
    }
}
