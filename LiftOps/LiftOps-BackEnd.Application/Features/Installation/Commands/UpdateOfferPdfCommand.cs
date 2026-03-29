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
    public class UpdateOfferPdfCommand : IRequest<Result<bool>>
    {
        public Guid OfferId { get; set; }
        public string OfferPdfPath { get; set; } = string.Empty;
    }

    public class UpdateOfferPdfCommandHandler : IRequestHandler<UpdateOfferPdfCommand, Result<bool>>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOfferPdfCommandHandler(
            IOfferRepository offerRepository,
            IUnitOfWork unitOfWork)
        {
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateOfferPdfCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var offer = await _offerRepository.GetByIdAsync(request.OfferId);

                if (offer == null)
                {
                    return Result<bool>.Failure("Offer not found.");
                }

                offer.OfferPdfPath = request.OfferPdfPath;
                _offerRepository.Update(offer);
                await _unitOfWork.Complete();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update offer PDF: {ex.Message}");
            }
        }
    }
}

