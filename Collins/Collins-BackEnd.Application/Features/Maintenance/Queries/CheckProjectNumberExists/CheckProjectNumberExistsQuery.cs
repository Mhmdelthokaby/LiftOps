using Collins_BackEnd.Application.Interfaces.Maintenance;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Queries.CheckProjectNumberExists
{
    public class CheckProjectNumberExistsQuery : IRequest<bool>
    {
        public string ProjectNumber { get; set; } = string.Empty;
    }

    public class CheckProjectNumberExistsQueryHandler : IRequestHandler<CheckProjectNumberExistsQuery, bool>
    {
        private readonly IMaintenanceService _maintenanceService;

        public CheckProjectNumberExistsQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<bool> Handle(CheckProjectNumberExistsQuery request, CancellationToken cancellationToken)
        {
            return await _maintenanceService.ProjectNumberExistsAsync(request.ProjectNumber);
        }
    }
}

