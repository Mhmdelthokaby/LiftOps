using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands;

public class RejectProjectCommand : IRequest<Result<Unit>>
{
    public Guid ProjectId { get; set; }
}

public class RejectProjectCommandHandler : IRequestHandler<RejectProjectCommand, Result<Unit>>
{
    private readonly IInstallationProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerStatusService _customerStatusService;

    public RejectProjectCommandHandler(
        IInstallationProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICustomerStatusService customerStatusService)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _customerStatusService = customerStatusService;
    }

    public async Task<Result<Unit>> Handle(RejectProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Use GetProjectWithDetailsAsync to ensure Customer is loaded
            var project = await _projectRepository.GetProjectWithDetailsAsync(request.ProjectId);
            
            if (project == null)
            {
                return Result<Unit>.Failure("Project not found.");
            }

            // Only allow rejecting projects that are pending inspection
            if (project.ProjectStatus != ProjectStatus.UnderInspectionAndQuotation)
            {
                return Result<Unit>.Failure("Only pending projects can be rejected.");
            }

            // Update project status to Rejected
            project.ProjectStatus = ProjectStatus.Rejected;

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
            return Result<Unit>.Failure($"Failed to reject project: {ex.Message}");
        }
    }
}

