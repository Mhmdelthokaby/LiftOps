using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Queries
{
    public class GetInspectionProjectsQuery : IRequest<Result<List<InspectionProjectDto>>>
    {
        public ProjectStatus? StatusFilter { get; set; }
    }

    public class GetInspectionProjectsQueryHandler : IRequestHandler<GetInspectionProjectsQuery, Result<List<InspectionProjectDto>>>
    {
        private readonly IInstallationProjectRepository _projectRepository;
        private readonly IQuotationRepository _quotationRepository;

        public GetInspectionProjectsQueryHandler(
            IInstallationProjectRepository projectRepository,
            IQuotationRepository quotationRepository)
        {
            _projectRepository = projectRepository;
            _quotationRepository = quotationRepository;
        }

        public async Task<Result<List<InspectionProjectDto>>> Handle(GetInspectionProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = await _projectRepository.GetProjectsWithStatusAsync();
                
                // Filter projects by status - default to UnderInspectionAndQuotation, Approved, and Rejected
                var filteredProjects = allProjects.Where(p =>
                {
                    if (request.StatusFilter.HasValue)
                    {
                        return p.ProjectStatus == request.StatusFilter.Value;
                    }
                    // Default: show projects under inspection, approved, and rejected
                    return p.ProjectStatus == ProjectStatus.UnderInspectionAndQuotation ||
                           p.ProjectStatus == ProjectStatus.Approved ||
                           p.ProjectStatus == ProjectStatus.Rejected;
                }).ToList();

                var dtos = new List<InspectionProjectDto>();

                foreach (var project in filteredProjects)
                {
                    var quotation = project.QuotationId.HasValue
                        ? await _quotationRepository.GetByIdAsync(project.QuotationId.Value)
                        : null;

                    var dto = new InspectionProjectDto
                    {
                        Id = project.Id,
                        CustomerId = project.CustomerId,
                        CustomerName = project.Customer?.Name ?? string.Empty,
                        CustomerPhone = project.Customer?.Phone ?? string.Empty,
                        CustomerEmail = project.Customer?.Email ?? string.Empty,
                        ProjectAddress = project.ProjectAddress ?? string.Empty,
                        GoogleMapsLink = project.GoogleMapsLink,
                        ProjectStatus = project.ProjectStatus.ToString(),
                        PitType = project.ShaftType ?? string.Empty,
                        PitWidth = project.ShaftWidth ?? 0,
                        PitDepth = project.ShaftDepth ?? 0,
                        LastFloorHeight = project.LastFloorHeight ?? 0,
                        HoleDepth = project.HoleDepth ?? 0,
                        TravelLength = project.TravelHeight ?? 0,
                        Notes = project.Notes,
                        CreatedAt = project.CreatedAt,
                        Quotation = quotation != null ? new QuotationDto
                        {
                            Id = quotation.Id,
                            ProjectId = quotation.ProjectId,
                            Price = quotation.Price,
                            DurationDays = quotation.DurationDays,
                            DurationNotes = quotation.DurationNotes,
                            Notes = quotation.Notes,
                            Status = quotation.Status.ToString(),
                            Attachments = quotation.Attachments.Select(a => new QuotationAttachmentDto
                            {
                                Id = a.Id,
                                FileName = a.FileName,
                                FilePath = a.FilePath,
                                ContentType = a.ContentType,
                                FileSize = a.FileSize
                            }).ToList(),
                            CreatedAt = quotation.CreatedAt
                        } : null
                    };

                    dtos.Add(dto);
                }

                return Result<List<InspectionProjectDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<InspectionProjectDto>>.Failure($"Failed to retrieve inspection projects: {ex.Message}");
            }
        }
    }
}

