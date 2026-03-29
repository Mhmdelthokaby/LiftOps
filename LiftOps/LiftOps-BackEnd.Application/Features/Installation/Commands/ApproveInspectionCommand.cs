using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using LiftOps_BackEnd.Domain.Interfaces.Installation;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class ApproveInspectionCommand : IRequest<Result<Unit>>
    {
        public Guid ProjectId { get; set; }
    }

    public class ApproveInspectionCommandHandler : IRequestHandler<ApproveInspectionCommand, Result<Unit>>
    {
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerStatusService _customerStatusService;

        public ApproveInspectionCommandHandler(
            IInstallationProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            ICustomerStatusService customerStatusService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _customerStatusService = customerStatusService;
        }

        public async Task<Result<Unit>> Handle(ApproveInspectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Use GetProjectWithDetailsAsync to ensure Customer is loaded
                var project = await _projectRepository.GetProjectWithDetailsAsync(request.ProjectId);
                
                if (project == null)
                {
                    return Result<Unit>.Failure("Project not found.");
                }

                // Only allow approval if project is in UnderInspectionAndQuotation status
                if (project.ProjectStatus != ProjectStatus.UnderInspectionAndQuotation)
                {
                    return Result<Unit>.Failure($"Project cannot be approved. Current status: {project.ProjectStatus}");
                }

                // Update project status to Approved
                project.ProjectStatus = ProjectStatus.Approved;

                _projectRepository.Update(project);
                await _unitOfWork.Complete();

                // Recalculate and update customer status based on all their projects
                if (project.Customer != null)
                {
                    await _customerStatusService.UpdateCustomerStatusAsync(project.CustomerId);
                }

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure($"Failed to approve inspection: {ex.Message}");
            }
        }
    }
}

