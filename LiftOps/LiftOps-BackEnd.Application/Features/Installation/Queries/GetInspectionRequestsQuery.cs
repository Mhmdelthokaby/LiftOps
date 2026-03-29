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
    public class GetInspectionRequestsQuery : IRequest<Result<List<InspectionRequestDto>>>
    {
        public InspectionStatus? StatusFilter { get; set; }
    }

    public class GetInspectionRequestsQueryHandler : IRequestHandler<GetInspectionRequestsQuery, Result<List<InspectionRequestDto>>>
    {
        private readonly IInspectionRequestRepository _repository;

        public GetInspectionRequestsQueryHandler(IInspectionRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<InspectionRequestDto>>> Handle(GetInspectionRequestsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var inspections = await _repository.GetInspectionsByStatusAsync(request.StatusFilter);

                var dtos = inspections.Select(i => new InspectionRequestDto
                {
                    Id = i.Id,
                    ClientId = i.ClientId,
                    ClientName = i.ClientName,
                    ClientPhone = i.ClientPhone,
                    ClientEmail = i.ClientEmail,
                    ProjectAddress = i.ProjectAddress,
                    GoogleMapsLink = i.GoogleMapsLink,
                    NumberOfElevatorsRequired = i.NumberOfElevatorsRequired,
                    ElevatorType = i.ElevatorType,
                    Status = i.Status.ToString(),
                    Notes = i.Notes,
                    CreatedAt = i.CreatedAt,
                    CreatedBy = i.CreatedBy,
                    ShaftType = i.ShaftType,
                    ShaftWidth = i.ShaftWidth,
                    ShaftDepth = i.ShaftDepth,
                    LastFloorHeight = i.LastFloorHeight,
                    PitDepth = i.PitDepth,
                    TravelHeight = i.TravelHeight,
                    TechnicalNotes = i.TechnicalNotes,
                    Offer = i.Offer != null ? new OfferDto
                    {
                        Id = i.Offer.Id,
                        InspectionRequestId = i.Offer.InspectionRequestId,
                        InstallationPricePerUnit = i.Offer.InstallationPricePerUnit,
                        TotalInstallationPrice = i.Offer.TotalInstallationPrice,
                        EstimatedStartDate = i.Offer.EstimatedStartDate,
                        EstimatedEndDate = i.Offer.EstimatedEndDate,
                        Status = i.Offer.Status.ToString(),
                        Notes = i.Offer.Notes,
                        OfferPdfPath = i.Offer.OfferPdfPath,
                        CreatedAt = i.Offer.CreatedAt,
                        CreatedBy = i.Offer.CreatedBy
                    } : null,
                    ConvertedToProjectId = i.ConvertedToProjectId
                }).ToList();

                return Result<List<InspectionRequestDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<InspectionRequestDto>>.Failure($"Failed to retrieve inspection requests: {ex.Message}");
            }
        }
    }
}

