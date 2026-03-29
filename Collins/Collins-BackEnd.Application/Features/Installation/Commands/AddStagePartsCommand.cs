using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class AddStagePartsCommand : IRequest<Result<Unit>>
    {
        public AddStagePartsDto Dto { get; set; } = null!;
    }

    public class AddStagePartsCommandHandler : IRequestHandler<AddStagePartsCommand, Result<Unit>>
    {
        private readonly IPartSelectionService _partService;

        public AddStagePartsCommandHandler(IPartSelectionService partService)
        {
            _partService = partService;
        }

        public async Task<Result<Unit>> Handle(AddStagePartsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _partService.AddPartsToStageAsync(request.Dto.StageId, request.Dto.Parts);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
