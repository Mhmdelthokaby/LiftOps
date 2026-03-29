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
    public class UpdateOfferCommand : IRequest<Result<bool>>
    {
        public Guid OfferId { get; set; }
        public UpdateOfferDto Dto { get; set; } = null!;
    }

    public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, Result<bool>>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOfferCommandHandler(
            IOfferRepository offerRepository,
            IUnitOfWork unitOfWork)
        {
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var offer = await _offerRepository.GetByIdAsync(request.OfferId);

                if (offer == null)
                {
                    return Result<bool>.Failure("Offer not found.");
                }

                // Only allow updates if offer is waiting for approval
                if (offer.Status != OfferStatus.WaitingForClientApproval)
                {
                    return Result<bool>.Failure("Only offers waiting for client approval can be updated.");
                }

                // Update fields if provided
                if (request.Dto.InstallationPricePerUnit.HasValue)
                {
                    offer.InstallationPricePerUnit = request.Dto.InstallationPricePerUnit.Value;
                }

                if (request.Dto.TotalInstallationPrice.HasValue)
                {
                    offer.TotalInstallationPrice = request.Dto.TotalInstallationPrice.Value;
                }

                if (request.Dto.EstimatedStartDate.HasValue)
                {
                    offer.EstimatedStartDate = request.Dto.EstimatedStartDate.Value;
                }

                if (request.Dto.EstimatedEndDate.HasValue)
                {
                    offer.EstimatedEndDate = request.Dto.EstimatedEndDate.Value;
                }

                if (request.Dto.Notes != null)
                {
                    offer.Notes = request.Dto.Notes;
                }

                // Validate dates
                if (offer.EstimatedEndDate < offer.EstimatedStartDate)
                {
                    return Result<bool>.Failure("Estimated end date must be after estimated start date.");
                }

                _offerRepository.Update(offer);
                await _unitOfWork.Complete();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update offer: {ex.Message}");
            }
        }
    }
}

