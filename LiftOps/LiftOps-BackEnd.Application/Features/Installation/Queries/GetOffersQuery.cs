using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Queries
{
    public class GetOffersQuery : IRequest<Result<List<OfferDto>>>
    {
        public OfferStatus? StatusFilter { get; set; }
    }

    public class GetOffersQueryHandler : IRequestHandler<GetOffersQuery, Result<List<OfferDto>>>
    {
        private readonly IOfferRepository _repository;

        public GetOffersQueryHandler(IOfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<OfferDto>>> Handle(GetOffersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var offers = await _repository.GetOffersByStatusAsync(request.StatusFilter);

                var dtos = offers.Select(o => new OfferDto
                {
                    Id = o.Id,
                    InspectionRequestId = o.InspectionRequestId,
                    InstallationPricePerUnit = o.InstallationPricePerUnit,
                    TotalInstallationPrice = o.TotalInstallationPrice,
                    EstimatedStartDate = o.EstimatedStartDate,
                    EstimatedEndDate = o.EstimatedEndDate,
                    Status = o.Status.ToString(),
                    Notes = o.Notes,
                    OfferPdfPath = o.OfferPdfPath,
                    CreatedAt = o.CreatedAt,
                    CreatedBy = o.CreatedBy
                }).ToList();

                return Result<List<OfferDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<OfferDto>>.Failure($"Failed to retrieve offers: {ex.Message}");
            }
        }
    }
}

