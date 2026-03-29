using LiftOps_BackEnd.Domain.Entities.Faults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Faults
{
    public interface IFaultService
    {
        Task<FaultTicket> CreateTicketAsync(FaultTicket ticket);
        Task AssignTechnicianAsync(Guid ticketId, Guid technicianId);
        Task AddSparePartsAsync(Guid ticketId, List<(Guid ItemId, int Qty)> parts);
        Task ResolveTicketAsync(Guid ticketId, string notes);
        Task<IReadOnlyList<FaultTicket>> GetOpenTicketsAsync();
        Task<IReadOnlyList<FaultTicket>> GetAllTicketsAsync();
    }
}
