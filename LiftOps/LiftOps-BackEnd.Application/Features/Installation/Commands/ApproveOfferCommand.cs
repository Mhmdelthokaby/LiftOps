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
    public class ApproveOfferCommand : IRequest<Result<bool>>
    {
        public ApproveOfferDto Dto { get; set; } = null!;
        public bool IsManagerOverride { get; set; } = false; // Manager can override
    }

    public class ApproveOfferCommandHandler : IRequestHandler<ApproveOfferCommand, Result<bool>>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IInspectionRequestRepository _inspectionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveOfferCommandHandler(
            IOfferRepository offerRepository,
            IInspectionRequestRepository inspectionRepository,
            IUnitOfWork unitOfWork)
        {
            _offerRepository = offerRepository;
            _inspectionRepository = inspectionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ApproveOfferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var offer = await _offerRepository.GetOfferWithInspectionAsync(request.Dto.OfferId);

                if (offer == null)
                {
                    return Result<bool>.Failure("Offer not found.");
                }

                if (offer.Status != OfferStatus.WaitingForClientApproval)
                {
                    return Result<bool>.Failure("Offer is not in a state that can be approved/rejected.");
                }

                // Update offer status
                offer.Status = request.Dto.IsAccepted ? OfferStatus.Accepted : OfferStatus.Rejected;
                if (!string.IsNullOrWhiteSpace(request.Dto.Notes))
                {
                    offer.Notes = string.IsNullOrWhiteSpace(offer.Notes) 
                        ? request.Dto.Notes 
                        : $"{offer.Notes}\n{request.Dto.Notes}";
                }

                // Update inspection status
                offer.InspectionRequest.Status = request.Dto.IsAccepted 
                    ? InspectionStatus.OfferAccepted 
                    : InspectionStatus.OfferRejected;

                _offerRepository.Update(offer);
                _inspectionRepository.Update(offer.InspectionRequest);
                await _unitOfWork.Complete();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update offer status: {ex.Message}");
            }
        }
    }
}

