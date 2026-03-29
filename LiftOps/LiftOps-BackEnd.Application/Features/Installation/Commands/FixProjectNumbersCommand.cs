using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class FixProjectNumbersCommand : IRequest<Result<int>>
    {
    }

    public class FixProjectNumbersCommandHandler : IRequestHandler<FixProjectNumbersCommand, Result<int>>
    {
        private readonly IInstallationProjectService _projectService;

        public FixProjectNumbersCommandHandler(IInstallationProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<Result<int>> Handle(FixProjectNumbersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var fixedCount = await _projectService.FixDuplicateProjectNumbersAsync();
                return Result<int>.Success(fixedCount);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }
    }
}

