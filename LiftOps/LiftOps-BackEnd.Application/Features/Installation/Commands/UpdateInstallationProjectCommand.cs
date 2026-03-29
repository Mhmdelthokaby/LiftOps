using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class UpdateInstallationProjectCommand : IRequest<Result<Unit>>
    {
        public UpdateProjectDto ProjectDto { get; set; } = null!;
    }

    public class UpdateInstallationProjectCommandHandler : IRequestHandler<UpdateInstallationProjectCommand, Result<Unit>>
    {
        private readonly IInstallationProjectService _projectService;

        public UpdateInstallationProjectCommandHandler(IInstallationProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<Result<Unit>> Handle(UpdateInstallationProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _projectService.UpdateProjectAsync(request.ProjectDto);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}

