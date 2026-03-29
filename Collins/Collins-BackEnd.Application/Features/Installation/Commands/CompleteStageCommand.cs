using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class CompleteStageCommand : IRequest<Result<Unit>>
    {
        public CompleteStageDto Dto { get; set; } = null!;
    }

    public class CompleteStageCommandHandler : IRequestHandler<CompleteStageCommand, Result<Unit>>
    {
        private readonly IStageService _stageService;

        public CompleteStageCommandHandler(IStageService stageService)
        {
            _stageService = stageService;
        }

        public async Task<Result<Unit>> Handle(CompleteStageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _stageService.CompleteStageAsync(
                    request.Dto.StageId, 
                    request.Dto.SupplyCost, 
                    request.Dto.Notes,
                    request.Dto.EndDate,
                    request.Dto.Price,
                    request.Dto.CollectPrice,
                    request.Dto.FreeMonths,
                    request.Dto.TechnicianRatings);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
