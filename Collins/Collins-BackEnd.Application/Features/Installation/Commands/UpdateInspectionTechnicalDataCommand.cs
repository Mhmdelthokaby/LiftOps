using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class UpdateInspectionTechnicalDataCommand : IRequest<Result<bool>>
    {
        public Guid InspectionId { get; set; }
        public UpdateInspectionTechnicalDataDto Dto { get; set; } = null!;
    }

    public class UpdateInspectionTechnicalDataCommandHandler : IRequestHandler<UpdateInspectionTechnicalDataCommand, Result<bool>>
    {
        private readonly IInspectionRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateInspectionTechnicalDataCommandHandler(
            IInspectionRequestRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateInspectionTechnicalDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var inspection = await _repository.GetByIdAsync(request.InspectionId);

                if (inspection == null)
                {
                    return Result<bool>.Failure("Inspection request not found.");
                }

                // Update technical data
                inspection.ShaftType = request.Dto.ShaftType ?? inspection.ShaftType;
                inspection.ShaftWidth = request.Dto.ShaftWidth ?? inspection.ShaftWidth;
                inspection.ShaftDepth = request.Dto.ShaftDepth ?? inspection.ShaftDepth;
                inspection.LastFloorHeight = request.Dto.LastFloorHeight ?? inspection.LastFloorHeight;
                inspection.PitDepth = request.Dto.PitDepth ?? inspection.PitDepth;
                inspection.TravelHeight = request.Dto.TravelHeight ?? inspection.TravelHeight;
                inspection.TechnicalNotes = request.Dto.TechnicalNotes ?? inspection.TechnicalNotes;

                // Update status to Inspected if not already
                if (inspection.Status == InspectionStatus.PendingInspection)
                {
                    inspection.Status = InspectionStatus.Inspected;
                }

                _repository.Update(inspection);
                await _unitOfWork.Complete();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update inspection technical data: {ex.Message}");
            }
        }
    }
}

