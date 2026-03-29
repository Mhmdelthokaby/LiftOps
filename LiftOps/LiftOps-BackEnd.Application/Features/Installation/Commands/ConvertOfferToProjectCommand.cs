using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class ConvertOfferToProjectCommand : IRequest<Result<Guid>>
    {
        public Guid OfferId { get; set; }
        public Guid InstallationAdminId { get; set; }
    }

    public class ConvertOfferToProjectCommandHandler : IRequestHandler<ConvertOfferToProjectCommand, Result<Guid>>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IInspectionRequestRepository _inspectionRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IElevatorRepository _elevatorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConvertOfferToProjectCommandHandler(
            IOfferRepository offerRepository,
            IInspectionRequestRepository inspectionRepository,
            ICustomerRepository customerRepository,
            IInstallationProjectRepository projectRepository,
            IElevatorRepository elevatorRepository,
            IUnitOfWork unitOfWork)
        {
            _offerRepository = offerRepository;
            _inspectionRepository = inspectionRepository;
            _customerRepository = customerRepository;
            _projectRepository = projectRepository;
            _elevatorRepository = elevatorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(ConvertOfferToProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var offer = await _offerRepository.GetOfferWithInspectionAsync(request.OfferId);

                if (offer == null)
                {
                    return Result<Guid>.Failure("Offer not found.");
                }

                if (offer.Status != OfferStatus.Accepted)
                {
                    return Result<Guid>.Failure("Only accepted offers can be converted to projects.");
                }

                if (offer.InspectionRequest.ConvertedToProjectId.HasValue)
                {
                    return Result<Guid>.Failure("This inspection has already been converted to a project.");
                }

                var inspection = offer.InspectionRequest;

                // Validate that technical data is present (recommended but not required)
                if (string.IsNullOrWhiteSpace(inspection.ShaftType) || 
                    !inspection.ShaftWidth.HasValue || 
                    !inspection.ShaftDepth.HasValue)
                {
                    // Warning: Technical data might be incomplete, but allow conversion
                    // You can make this stricter if needed
                }

                // Handle customer: Use linked client if exists, otherwise find or create
                Customer? customer = null;

                // Priority 1: If inspection is linked to existing client, use it
                if (inspection.ClientId.HasValue)
                {
                    customer = await _customerRepository.GetByIdAsync(inspection.ClientId.Value);
                    if (customer == null)
                    {
                        return Result<Guid>.Failure("Linked client not found. Please contact support.");
                    }
                }
                else
                {
                    // Priority 2: Try to find existing customer by phone
                    customer = await _customerRepository.GetCustomerByPhoneAsync(inspection.ClientPhone);
                    
                    if (customer == null)
                    {
                        // Priority 3: Try to find by email
                        var allCustomers = await _customerRepository.ListAllAsync();
                        customer = allCustomers.FirstOrDefault(c => 
                            !string.IsNullOrWhiteSpace(c.Email) &&
                            c.Email.ToLower() == inspection.ClientEmail.ToLower());
                    }

                    // Priority 4: Create new customer (only when offer is accepted)
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            Name = inspection.ClientName,
                            Phone = inspection.ClientPhone,
                            Email = inspection.ClientEmail,
                            Address = inspection.ProjectAddress,
                            GoogleMapsLink = inspection.GoogleMapsLink,
                            ProjectNumber = string.Empty // Will be auto-generated
                        };
                        _customerRepository.Add(customer);
                        await _unitOfWork.Complete();
                    }
                    else
                    {
                        // Update customer info if needed
                        if (string.IsNullOrWhiteSpace(customer.Address))
                            customer.Address = inspection.ProjectAddress;
                        if (string.IsNullOrWhiteSpace(customer.GoogleMapsLink))
                            customer.GoogleMapsLink = inspection.GoogleMapsLink;
                        _customerRepository.Update(customer);
                    }
                }

                // Generate unique project number
                string projectNumber;
                int counter = 1;
                var allProjects = await _projectRepository.ListAllAsync();
                do
                {
                    projectNumber = $"PRJ-{DateTime.UtcNow:yyyyMMdd}-{counter:D4}";
                    counter++;
                } while (allProjects.Any(p => p.ProjectNumber == projectNumber));

                // Create project
                var project = new InstallationProject
                {
                    CustomerId = customer.Id,
                    InstallationAdminId = request.InstallationAdminId,
                    ProjectNumber = projectNumber,
                    ProjectAddress = inspection.ProjectAddress,
                    GoogleMapsLink = inspection.GoogleMapsLink,
                    InstallationPricePerUnit = offer.InstallationPricePerUnit,
                    TotalPrice = offer.TotalInstallationPrice,
                    ContractDate = DateTime.UtcNow,
                    InstallationStartDate = offer.EstimatedStartDate,
                    ExpectedFinishDate = offer.EstimatedEndDate,
                    Notes = inspection.Notes,
                    
                    // Technical data from inspection
                    ShaftType = inspection.ShaftType,
                    ShaftWidth = inspection.ShaftWidth,
                    ShaftDepth = inspection.ShaftDepth,
                    LastFloorHeight = inspection.LastFloorHeight,
                    PitDepth = inspection.PitDepth,
                    TravelHeight = inspection.TravelHeight,
                    TechnicalNotes = inspection.TechnicalNotes,
                    
                    // Link to inspection
                    ConvertedFromInspectionId = inspection.Id
                };

                _projectRepository.Add(project);
                await _unitOfWork.Complete();

                // Create elevators based on inspection
                // Parse ElevatorType from string to enum
                ElevatorType elevatorType = ElevatorType.WithMachineRoom;
                if (!string.IsNullOrWhiteSpace(inspection.ElevatorType))
                {
                    Enum.TryParse<ElevatorType>(inspection.ElevatorType, true, out elevatorType);
                }

                // Parse PitType from ShaftType
                PitType pitType = PitType.Concrete;
                if (!string.IsNullOrWhiteSpace(inspection.ShaftType))
                {
                    Enum.TryParse<PitType>(inspection.ShaftType, true, out pitType);
                }

                for (int i = 0; i < inspection.NumberOfElevatorsRequired; i++)
                {
                    var elevator = new Elevator
                    {
                        ProjectId = project.Id,
                        ElevatorType = elevatorType,
                        FloorsCount = 0, // Will be updated later if needed
                        StopsCount = 0, // Will be updated later if needed
                        NumberOfStops = 0, // Will be updated later if needed
                        NumberOfFloors = 0, // Will be updated later if needed
                        NumberOfElevators = 1, // Each elevator is a separate entity
                        PitType = pitType,
                        PitWidth = inspection.ShaftWidth ?? 0,
                        PitDepth = inspection.PitDepth ?? 0,
                        LastFloorHeight = inspection.LastFloorHeight ?? 0,
                        HoleDepth = 0, // Not in InspectionRequest, will be updated later
                        TravelLength = inspection.TravelHeight ?? 0,
                        Notes = inspection.TechnicalNotes,
                        Price = 0 // Will be set from offer
                    };
                    _elevatorRepository.Add(elevator);
                }

                // Update inspection to mark as converted
                inspection.ConvertedToProjectId = project.Id;
                _inspectionRepository.Update(inspection);
                await _unitOfWork.Complete();

                return Result<Guid>.Success(project.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to convert offer to project: {ex.Message}");
            }
        }
    }
}
