using Collins_BackEnd.Domain.Entities.Faults;
using Collins_BackEnd.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Domain.Interfaces.Faults
{
    public interface IFaultRepository : IGenericRepository<FaultTicket>
    {
        Task<IReadOnlyList<FaultTicket>> GetOpenTicketsAsync();
        Task<IReadOnlyList<FaultTicket>> GetTicketsByTechnicianAsync(System.Guid technicianId);
    }
}
