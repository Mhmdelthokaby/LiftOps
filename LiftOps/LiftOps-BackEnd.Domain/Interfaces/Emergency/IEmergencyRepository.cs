using LiftOps_BackEnd.Domain.Entities.Emergency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Emergency
{
    public interface IEmergencyRepository : IGenericRepository<EmergencyTicket>
    {
        Task<IReadOnlyList<EmergencyTicket>> GetOpenTicketsAsync();
        Task<IReadOnlyList<EmergencyTicket>> GetTicketsByStatusAsync(EmergencyStatus status);
        Task<IReadOnlyList<EmergencyTicket>> GetTicketsByTechnicianAsync(Guid technicianId);
        Task<int> GetNextTicketNumberAsync();
    }
}

