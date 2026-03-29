using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class CreateOfferCommand : IRequest<Result<Guid>>
    {
        public CreateOfferDto Dto { get; set; } = null!;
    }

    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Result<Guid>>
    {
        private readonly IInspectionRequestRepository _inspectionRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOfferCommandHandler(
            IInspectionRequestRepository inspectionRepository,
            IOfferRepository offerRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionRepository = inspectionRepository;
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var inspection = await _inspectionRepository.GetByIdAsync(request.Dto.InspectionRequestId);

                if (inspection == null)
                {
                    return Result<Guid>.Failure("Inspection request not found.");
                }

                if (inspection.Status != InspectionStatus.Inspected)
                {
                    return Result<Guid>.Failure("Inspection must be completed before creating an offer.");
                }

                // Check if offer already exists
                var existingOffer = await _offerRepository.GetOfferByInspectionRequestIdAsync(request.Dto.InspectionRequestId);
                if (existingOffer != null)
                {
                    return Result<Guid>.Failure("An offer already exists for this inspection request.");
                }

                var offer = new Offer
                {
                    InspectionRequestId = request.Dto.InspectionRequestId,
                    InstallationPricePerUnit = request.Dto.InstallationPricePerUnit,
                    TotalInstallationPrice = request.Dto.TotalInstallationPrice,
                    EstimatedStartDate = request.Dto.EstimatedStartDate,
                    EstimatedEndDate = request.Dto.EstimatedEndDate,
                    Notes = request.Dto.Notes,
                    Status = OfferStatus.WaitingForClientApproval
                };

                _offerRepository.Add(offer);
                
                // Update inspection status
                inspection.Status = InspectionStatus.OfferSent;
                _inspectionRepository.Update(inspection);

                await _unitOfWork.Complete();

                return Result<Guid>.Success(offer.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create offer: {ex.Message}");
            }
        }
    }
}

