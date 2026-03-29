using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Queries
{
    public class GetInspectionRequestDetailsQuery : IRequest<Result<InspectionRequestDto>>
    {
        public Guid InspectionId { get; set; }
    }

    public class GetInspectionRequestDetailsQueryHandler : IRequestHandler<GetInspectionRequestDetailsQuery, Result<InspectionRequestDto>>
    {
        private readonly IInspectionRequestRepository _repository;

        public GetInspectionRequestDetailsQueryHandler(IInspectionRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<InspectionRequestDto>> Handle(GetInspectionRequestDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var inspection = await _repository.GetInspectionWithOfferAsync(request.InspectionId);

                if (inspection == null)
                {
                    return Result<InspectionRequestDto>.Failure("Inspection request not found.");
                }

                var dto = new InspectionRequestDto
                {
                    Id = inspection.Id,
                    ClientId = inspection.ClientId,
                    ClientName = inspection.ClientName,
                    ClientPhone = inspection.ClientPhone,
                    ClientEmail = inspection.ClientEmail,
                    ProjectAddress = inspection.ProjectAddress,
                    GoogleMapsLink = inspection.GoogleMapsLink,
                    NumberOfElevatorsRequired = inspection.NumberOfElevatorsRequired,
                    ElevatorType = inspection.ElevatorType,
                    Status = inspection.Status.ToString(),
                    Notes = inspection.Notes,
                    CreatedAt = inspection.CreatedAt,
                    CreatedBy = inspection.CreatedBy,
                    ShaftType = inspection.ShaftType,
                    ShaftWidth = inspection.ShaftWidth,
                    ShaftDepth = inspection.ShaftDepth,
                    LastFloorHeight = inspection.LastFloorHeight,
                    PitDepth = inspection.PitDepth,
                    TravelHeight = inspection.TravelHeight,
                    TechnicalNotes = inspection.TechnicalNotes,
                    Offer = inspection.Offer != null ? new OfferDto
                    {
                        Id = inspection.Offer.Id,
                        InspectionRequestId = inspection.Offer.InspectionRequestId,
                        InstallationPricePerUnit = inspection.Offer.InstallationPricePerUnit,
                        TotalInstallationPrice = inspection.Offer.TotalInstallationPrice,
                        EstimatedStartDate = inspection.Offer.EstimatedStartDate,
                        EstimatedEndDate = inspection.Offer.EstimatedEndDate,
                        Status = inspection.Offer.Status.ToString(),
                        Notes = inspection.Offer.Notes,
                        OfferPdfPath = inspection.Offer.OfferPdfPath,
                        CreatedAt = inspection.Offer.CreatedAt,
                        CreatedBy = inspection.Offer.CreatedBy
                    } : null,
                    ConvertedToProjectId = inspection.ConvertedToProjectId
                };

                return Result<InspectionRequestDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<InspectionRequestDto>.Failure($"Failed to retrieve inspection request details: {ex.Message}");
            }
        }
    }
}

