using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class CreateInspectionProjectCommand : IRequest<Result<Guid>>
    {
        public CreateInspectionProjectDto Dto { get; set; } = null!;
        public Guid InstallationAdminId { get; set; }
    }

    public class CreateInspectionProjectCommandHandler : IRequestHandler<CreateInspectionProjectCommand, Result<Guid>>
    {
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInstallationProjectService _projectService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInspectionProjectCommandHandler(
            IInstallationProjectRepository projectRepository,
            ICustomerRepository customerRepository,
            IInstallationProjectService projectService,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _customerRepository = customerRepository;
            _projectService = projectService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateInspectionProjectCommand request, CancellationToken cancellationToken)
        {
            if (request.InstallationAdminId == Guid.Empty)
                return Result<Guid>.Failure(IdentityErrors.AuthenticatedUserIdentityRequired);

            try
            {
                Customer? customer = null;

                // If CustomerId is provided, use existing customer
                if (request.Dto.CustomerId.HasValue)
                {
                    customer = await _customerRepository.GetByIdAsync(request.Dto.CustomerId.Value);
                    if (customer == null)
                    {
                        return Result<Guid>.Failure("Specified customer not found.");
                    }
                }
                else
                {
                    // Search for existing customer by phone/email
                    if (!string.IsNullOrWhiteSpace(request.Dto.CustomerPhone))
                    {
                        customer = await _projectService.GetCustomerByContactAsync(
                            request.Dto.CustomerEmail ?? string.Empty,
                            request.Dto.CustomerPhone);
                    }

                    // If customer not found, create new customer with status "PendingInspectionQuotation"
                    if (customer == null)
                    {
                        if (string.IsNullOrWhiteSpace(request.Dto.CustomerPhone))
                        {
                            return Result<Guid>.Failure("Customer phone is required.");
                        }

                        customer = new Customer
                        {
                            Name = request.Dto.CustomerName ?? string.Empty,
                            Phone = request.Dto.CustomerPhone,
                            Email = request.Dto.CustomerEmail ?? string.Empty,
                            Address = request.Dto.CustomerAddress ?? string.Empty,
                            Status = CustomerStatus.PendingInspectionQuotation
                        };

                        _customerRepository.Add(customer);
                        await _unitOfWork.Complete();
                    }
                }

                // Create project with status "UnderInspectionAndQuotation"
                var project = new InstallationProject
                {
                    CustomerId = customer.Id,
                    InstallationAdminId = request.InstallationAdminId,
                    ProjectAddress = request.Dto.ProjectAddress,
                    GoogleMapsLink = request.Dto.GoogleMapsLink,
                    ProjectStatus = ProjectStatus.UnderInspectionAndQuotation,
                    // Required pit fields
                    ShaftType = request.Dto.PitType, // pitType
                    ShaftWidth = request.Dto.PitWidth, // pitWidth
                    ShaftDepth = request.Dto.PitDepth, // pitDepth
                    LastFloorHeight = request.Dto.LastFloorHeight,
                    PitDepth = request.Dto.PitDepth,
                    HoleDepth = request.Dto.HoleDepth,
                    TravelHeight = request.Dto.TravelLength, // travelLength
                    Notes = request.Dto.Notes,
                    // Set default values for required fields
                    InstallationPricePerUnit = 0,
                    TotalPrice = 0,
                    ContractDate = DateTime.UtcNow
                };

                // Generate project number using service
                // The service will handle project number generation
                // We'll set it to empty and let the service generate it
                project.ProjectNumber = string.Empty;
                
                // Use the service to create the project (it handles project number generation)
                // But we need to set status after, so we'll do it manually
                var allProjects = await _projectRepository.ListAllAsync();
                int maxNumber = 0;
                foreach (var proj in allProjects)
                {
                    if (string.IsNullOrWhiteSpace(proj.ProjectNumber)) continue;
                    var parts = proj.ProjectNumber.Split('-');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int num))
                    {
                        if (num > maxNumber) maxNumber = num;
                    }
                }
                int nextNumber = maxNumber + 1;
                project.ProjectNumber = $"PR-{nextNumber:D2}";
                
                // Ensure uniqueness
                while (allProjects.Any(p => !string.IsNullOrWhiteSpace(p.ProjectNumber) && 
                    p.ProjectNumber.Equals(project.ProjectNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    nextNumber++;
                    project.ProjectNumber = $"PR-{nextNumber:D2}";
                }

                _projectRepository.Add(project);
                await _unitOfWork.Complete();

                return Result<Guid>.Success(project.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create inspection project: {ex.Message}");
            }
        }
    }
}

