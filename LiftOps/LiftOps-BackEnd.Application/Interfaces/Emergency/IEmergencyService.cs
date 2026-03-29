using LiftOps_BackEnd.Domain.Entities.Emergency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Emergency
{
    public interface IEmergencyService
    {
        Task<EmergencyTicket> CreateTicketAsync(EmergencyTicket ticket);
        Task<EmergencyTicket> UpdateTicketAsync(Guid ticketId, EmergencyTicket ticket);
        Task DeleteTicketAsync(Guid ticketId);
        Task<EmergencyTicket?> GetTicketByIdAsync(Guid ticketId);
        Task<IReadOnlyList<EmergencyTicket>> GetAllTicketsAsync();
        Task<IReadOnlyList<EmergencyTicket>> GetOpenTicketsAsync();
        Task<IReadOnlyList<EmergencyTicket>> GetTicketsByTechnicianAsync(Guid technicianId);
        Task AssignTechnicianAsync(Guid ticketId, Guid technicianId);
        Task ResolveTicketAsync(Guid ticketId, string? notes = null);
    }
}

