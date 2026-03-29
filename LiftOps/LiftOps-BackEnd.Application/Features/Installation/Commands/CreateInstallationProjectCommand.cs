using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class CreateInstallationProjectCommand : IRequest<Result<Guid>>
    {
        public CreateProjectDto ProjectDto { get; set; } = null!;
        public Guid InstallationAdminId { get; set; } // Set from Controller
    }

    public class CreateInstallationProjectCommandHandler : IRequestHandler<CreateInstallationProjectCommand, Result<Guid>>
    {
        private readonly IInstallationProjectService _projectService;
        private readonly IElevatorService _elevatorService;
        private readonly IMapper _mapper;

        public CreateInstallationProjectCommandHandler(
            IInstallationProjectService projectService,
            IElevatorService elevatorService,
            IMapper mapper)
        {
            _projectService = projectService;
            _elevatorService = elevatorService;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateInstallationProjectCommand request, CancellationToken cancellationToken)
        {
            // Map Customer
            var customer = _mapper.Map<Customer>(request.ProjectDto.Customer);
            
            // Map Project
            var project = _mapper.Map<InstallationProject>(request.ProjectDto.Contract);
            project.Customer = customer;
            // Use ProjectNumber from CustomerDto if provided, otherwise will be auto-generated
            project.ProjectNumber = request.ProjectDto.Customer.ProjectNumber ?? string.Empty;
            // Set GoogleMapsLink from contract if provided, otherwise from customer
            project.GoogleMapsLink = request.ProjectDto.Contract.GoogleMapsLink ?? request.ProjectDto.Customer.GoogleMapsLink;
            project.InstallationAdminId = request.InstallationAdminId;
            // All new projects must go through inspection/quotation approval
            // Set project status to UnderInspectionAndQuotation - requires inspection and quotation approval
            project.ProjectStatus = ProjectStatus.UnderInspectionAndQuotation;
            // QuotationId will be set when quotation is created
            project.QuotationId = null;

            try
            {
                var createdProject = await _projectService.CreateProjectAsync(project);

                // Add Elevators
                foreach (var elevatorDto in request.ProjectDto.Elevators)
                {
                    var elevator = _mapper.Map<Elevator>(elevatorDto);
                    elevator.ProjectId = createdProject.Id;
                    await _elevatorService.AddElevatorAsync(elevator);
                }

                return Result<Guid>.Success(createdProject.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
