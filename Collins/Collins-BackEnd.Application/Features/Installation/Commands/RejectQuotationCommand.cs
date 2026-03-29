using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class RejectQuotationCommand : IRequest<Result<Unit>>
    {
        public ApproveRejectQuotationDto Dto { get; set; } = null!;
    }

    public class RejectQuotationCommandHandler : IRequestHandler<RejectQuotationCommand, Result<Unit>>
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerStatusService _customerStatusService;

        public RejectQuotationCommandHandler(
            IQuotationRepository quotationRepository,
            IInstallationProjectRepository projectRepository,
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            ICustomerStatusService customerStatusService)
        {
            _quotationRepository = quotationRepository;
            _projectRepository = projectRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _customerStatusService = customerStatusService;
        }

        public async Task<Result<Unit>> Handle(RejectQuotationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var quotation = await _quotationRepository.GetQuotationWithProjectAsync(request.Dto.QuotationId);
                if (quotation == null)
                {
                    return Result<Unit>.Failure("Quotation not found.");
                }

                if (quotation.Status != QuotationStatus.Pending)
                {
                    return Result<Unit>.Failure("Only pending quotations can be rejected.");
                }

                var project = quotation.Project;
                if (project == null)
                {
                    return Result<Unit>.Failure("Project not found.");
                }

                if (project.ProjectStatus != ProjectStatus.UnderInspectionAndQuotation)
                {
                    return Result<Unit>.Failure("Only projects under inspection and quotation can be rejected.");
                }

                // Update quotation status
                quotation.Status = QuotationStatus.Rejected;
                if (!string.IsNullOrWhiteSpace(request.Dto.Notes))
                {
                    quotation.Notes = (quotation.Notes ?? string.Empty) + "\n" + request.Dto.Notes;
                }
                _quotationRepository.Update(quotation);

                // Update project status to Rejected
                project.ProjectStatus = ProjectStatus.Rejected;
                _projectRepository.Update(project);
                await _unitOfWork.Complete();

                // Recalculate and update customer status based on all their projects
                // This ensures proper status calculation (Rejected only if all projects are rejected)
                await _customerStatusService.UpdateCustomerStatusAsync(project.CustomerId);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure($"Failed to reject quotation: {ex.Message}");
            }
        }
    }
}

