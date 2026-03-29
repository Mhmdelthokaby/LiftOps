using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Domain.Entities.Maintenance;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.CreateMaintenanceProject
{
    public class CreateMaintenanceProjectCommand : IRequest<Result<Guid>>
    {
        public CreateMaintenanceProjectDto ProjectDto { get; set; } = null!;
    }

    public class CreateMaintenanceProjectCommandHandler : IRequestHandler<CreateMaintenanceProjectCommand, Result<Guid>>
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IInstallationProjectService _installationProjectService;
        private readonly IMapper _mapper;

        public CreateMaintenanceProjectCommandHandler(
            IMaintenanceService maintenanceService,
            IInstallationProjectService installationProjectService,
            IMapper mapper)
        {
            _maintenanceService = maintenanceService;
            _installationProjectService = installationProjectService;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateMaintenanceProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if project number exists
                if (!string.IsNullOrWhiteSpace(request.ProjectDto.Contract.ProjectNumber))
                {
                    if (await _maintenanceService.ProjectNumberExistsAsync(request.ProjectDto.Contract.ProjectNumber))
                    {
                        return Result<Guid>.Failure($"Project number '{request.ProjectDto.Contract.ProjectNumber}' already exists.");
                    }
                }

                // Get or create customer (similar to installation project creation)
                var customer = await _installationProjectService.GetCustomerByContactAsync(
                    request.ProjectDto.Customer.Email,
                    request.ProjectDto.Customer.Phone);

                if (customer == null)
                {
                    // Create new customer
                    customer = _mapper.Map<Customer>(request.ProjectDto.Customer);
                }
                else
                {
                    // Update customer info if needed
                    customer.Name = request.ProjectDto.Customer.Name;
                    customer.Address = request.ProjectDto.Customer.Address;
                    customer.City = request.ProjectDto.Customer.City;
                    if (!string.IsNullOrWhiteSpace(request.ProjectDto.Customer.Email))
                        customer.Email = request.ProjectDto.Customer.Email;
                }

                // Create maintenance contract
                var contract = new MaintenanceContract
                {
                    Customer = customer,
                    ProjectNumber = request.ProjectDto.Contract.ProjectNumber,
                    // Fallback logic: If project address/city is not provided, use customer's address/city
                    ProjectAddress = !string.IsNullOrWhiteSpace(request.ProjectDto.Contract.ProjectAddress) 
                        ? request.ProjectDto.Contract.ProjectAddress 
                        : customer.Address,
                    City = !string.IsNullOrWhiteSpace(request.ProjectDto.Contract.City) && request.ProjectDto.Contract.City != "القاهرة الجديدة"
                        ? request.ProjectDto.Contract.City 
                        : customer.City,
                    GoogleMapsLink = request.ProjectDto.Contract.GoogleMapsLink,
                    StartDate = request.ProjectDto.Contract.StartDate,
                    EndDate = request.ProjectDto.Contract.EndDate,
                    PricePerMonth = request.ProjectDto.Contract.PricePerMonth,
                    FreeMonths = request.ProjectDto.Contract.FreeMonths,
                    TechnicianId = request.ProjectDto.Contract.TechnicianId,
                    Status = MaintenanceContractStatus.Active,
                    IsFromInstallation = false
                };

                // Map elevators
                var elevators = request.ProjectDto.Elevators.Select(e => new MaintenanceElevator
                {
                    Type = e.Type,
                    NumberOfStops = e.NumberOfStops,
                    NumberOfFloors = e.NumberOfFloors,
                    Status = MaintenanceElevatorStatus.Active
                }).ToList();

                // Create project with elevators
                var createdContract = await _maintenanceService.CreateMaintenanceProjectAsync(contract, elevators);

                return Result<Guid>.Success(createdContract.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}

