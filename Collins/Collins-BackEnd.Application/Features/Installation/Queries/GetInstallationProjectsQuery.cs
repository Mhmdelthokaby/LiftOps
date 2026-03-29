using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Queries
{
    public class GetInstallationProjectsQuery : IRequest<Result<IReadOnlyList<InstallationProjectDto>>>
    {
        public ProjectStatus? StatusFilter { get; set; }
    }

    public class GetInstallationProjectsQueryHandler : IRequestHandler<GetInstallationProjectsQuery, Result<IReadOnlyList<InstallationProjectDto>>>
    {
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetInstallationProjectsQueryHandler(IInstallationProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<InstallationProjectDto>>> Handle(GetInstallationProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetProjectsWithStatusAsync();
            
            // If status filter is provided, filter by that status
            if (request.StatusFilter.HasValue)
            {
                var filteredProjects = projects.Where(p => p.ProjectStatus == request.StatusFilter.Value).ToList();
                var dtos = _mapper.Map<IReadOnlyList<InstallationProjectDto>>(filteredProjects);
                return Result<IReadOnlyList<InstallationProjectDto>>.Success(dtos);
            }
            
            // Default behavior: Show approved/active projects and existing projects (without quotations)
            var visibleProjects = projects.Where(p => 
                p.ProjectStatus == ProjectStatus.Approved || 
                p.ProjectStatus == ProjectStatus.Active ||
                (p.QuotationId == null)).ToList(); // Show existing projects without quotations
            var defaultDtos = _mapper.Map<IReadOnlyList<InstallationProjectDto>>(visibleProjects);
            return Result<IReadOnlyList<InstallationProjectDto>>.Success(defaultDtos);
        }
    }
}
