using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class UpdateStageCommand : IRequest<Result<Unit>>
    {
        public UpdateStageDto Dto { get; set; } = null!;
    }

    public class UpdateStageCommandHandler : IRequestHandler<UpdateStageCommand, Result<Unit>>
    {
        private readonly IStageService _stageService;

        public UpdateStageCommandHandler(IStageService stageService)
        {
            _stageService = stageService;
        }

        public async Task<Result<Unit>> Handle(UpdateStageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _stageService.UpdateStageAsync(
                    request.Dto.StageId, 
                    request.Dto.Parts, 
                    request.Dto.Notes, 
                    request.Dto.SupplyCost,
                    request.Dto.StagePrice,
                    request.Dto.EndDate,
                    request.Dto.TechnicianIds);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}

