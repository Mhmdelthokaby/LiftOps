using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Interfaces;
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
    public class CreateQuotationCommand : IRequest<Result<Guid>>
    {
        public CreateQuotationDto Dto { get; set; } = null!;
    }

    public class CreateQuotationCommandHandler : IRequestHandler<CreateQuotationCommand, Result<Guid>>
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTenantService _currentTenantService;

        public CreateQuotationCommandHandler(
            IQuotationRepository quotationRepository,
            IInstallationProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            ICurrentTenantService currentTenantService)
        {
            _quotationRepository = quotationRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _currentTenantService = currentTenantService;
        }

        public async Task<Result<Guid>> Handle(CreateQuotationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(request.Dto.ProjectId);
                if (project == null)
                {
                    return Result<Guid>.Failure("Project not found.");
                }

                if (project.ProjectStatus != ProjectStatus.UnderInspectionAndQuotation)
                {
                    return Result<Guid>.Failure("Quotation can only be created for projects under inspection and quotation.");
                }

                // Check if quotation already exists
                var existingQuotation = await _quotationRepository.GetQuotationByProjectIdAsync(request.Dto.ProjectId);
                if (existingQuotation != null)
                {
                    return Result<Guid>.Failure("A quotation already exists for this project.");
                }

                var quotation = new Quotation
                {
                    ProjectId = request.Dto.ProjectId,
                    Price = request.Dto.Price,
                    DurationDays = request.Dto.DurationDays,
                    DurationNotes = request.Dto.DurationNotes,
                    Notes = request.Dto.Notes,
                    Status = QuotationStatus.Pending
                };

                // Add attachments if provided
                if (request.Dto.Attachments != null && request.Dto.Attachments.Any())
                {
                    foreach (var attachmentDto in request.Dto.Attachments)
                    {
                        quotation.Attachments.Add(new QuotationAttachment
                        {
                            FileName = attachmentDto.FileName,
                            FilePath = EnsureTenantScopedPath(attachmentDto.FilePath),
                            ContentType = attachmentDto.ContentType,
                            FileSize = attachmentDto.FileSize
                        });
                    }
                }

                _quotationRepository.Add(quotation);
                
                // Link quotation to project
                project.QuotationId = quotation.Id;
                _projectRepository.Update(project);

                await _unitOfWork.Complete();

                return Result<Guid>.Success(quotation.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create quotation: {ex.Message}");
            }
        }

        private string EnsureTenantScopedPath(string path)
        {
            var normalized = path.Replace('\\', '/').TrimStart('/');
            if (_currentTenantService.CompanyId == null || _currentTenantService.CompanyId == Guid.Empty)
            {
                return normalized;
            }

            var tenantPrefix = _currentTenantService.CompanyId.Value.ToString();
            return normalized.StartsWith($"{tenantPrefix}/", StringComparison.OrdinalIgnoreCase)
                ? normalized
                : $"{tenantPrefix}/{normalized}";
        }
    }
}

