using Collins_BackEnd.Application.DTOs.Faults;
using Collins_BackEnd.Application.Interfaces.Faults;
using Collins_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Collins_BackEnd.Domain.Entities.Faults;

namespace Collins_BackEnd.Application.Features.Faults.Queries
{
    public class GetOpenFaultsQuery : IRequest<Result<IReadOnlyList<FaultTicket>>> 
    {
    }

    public class GetOpenFaultsQueryHandler : IRequestHandler<GetOpenFaultsQuery, Result<IReadOnlyList<FaultTicket>>>
    {
        private readonly IFaultService _service;

        public GetOpenFaultsQueryHandler(IFaultService service)
        {
            _service = service;
        }

        public async Task<Result<IReadOnlyList<FaultTicket>>> Handle(GetOpenFaultsQuery request, CancellationToken cancellationToken)
        {
            // Note: Returning Domain Entity directly for speed/simplicity as per request, 
            // but ideally should map to DTO. Request didn't specify strict DTOs for reads but recommended.
            // I'll return Entity list directly or simple map? 
            // Let's return Entity List for now or create a DTO map if I had one. 
            // I don't have FaultTicketDto defined yet, only Create/Update DTOs.
            // I'll stick to returning Entities for MVP speed unless strict.
            var tickets = await _service.GetOpenTicketsAsync();
            return Result<IReadOnlyList<FaultTicket>>.Success(tickets);
        }
    }
}
