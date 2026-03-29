using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class StartStageCommand : IRequest<Result<Unit>>
    {
        public StartStageDto Dto { get; set; } = null!;
    }

    public class StartStageCommandHandler : IRequestHandler<StartStageCommand, Result<Unit>>
    {
        private readonly IStageService _stageService;

        public StartStageCommandHandler(IStageService stageService)
        {
            _stageService = stageService;
        }

        public async Task<Result<Unit>> Handle(StartStageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _stageService.StartStageAsync(request.Dto.StageId, request.Dto.StartDate ?? DateTime.UtcNow);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
