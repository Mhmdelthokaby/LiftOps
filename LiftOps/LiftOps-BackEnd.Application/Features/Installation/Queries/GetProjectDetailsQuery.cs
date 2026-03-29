using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Queries
{
    public class GetProjectDetailsQuery : IRequest<Result<InstallationProjectDto>>
    {
        public Guid ProjectId { get; set; }
    }

    public class GetProjectDetailsQueryHandler : IRequestHandler<GetProjectDetailsQuery, Result<InstallationProjectDto>>
    {
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectDetailsQueryHandler(IInstallationProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<Result<InstallationProjectDto>> Handle(GetProjectDetailsQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(request.ProjectId);
            if (project == null) return Result<InstallationProjectDto>.Failure("Project not found");

            var dto = _mapper.Map<InstallationProjectDto>(project);
            return Result<InstallationProjectDto>.Success(dto);
        }
    }
}
