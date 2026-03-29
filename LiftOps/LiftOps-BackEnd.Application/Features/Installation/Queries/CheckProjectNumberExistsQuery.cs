using LiftOps_BackEnd.Application.Interfaces.Installation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Queries
{
    public class CheckProjectNumberExistsQuery : IRequest<bool>
    {
        public string ProjectNumber { get; set; } = string.Empty;
    }

    public class CheckProjectNumberExistsQueryHandler : IRequestHandler<CheckProjectNumberExistsQuery, bool>
    {
        private readonly IInstallationProjectService _projectService;

        public CheckProjectNumberExistsQueryHandler(IInstallationProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<bool> Handle(CheckProjectNumberExistsQuery request, CancellationToken cancellationToken)
        {
            return await _projectService.ProjectNumberExistsAsync(request.ProjectNumber);
        }
    }
}

