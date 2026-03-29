using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using System.Linq;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Queries.ListChecklistItems;

public record ListChecklistItemsQuery(bool IncludeInactive = false) : IRequest<List<MaintenanceChecklistItemDto>>;

public class ListChecklistItemsQueryHandler : IRequestHandler<ListChecklistItemsQuery, List<MaintenanceChecklistItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListChecklistItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MaintenanceChecklistItemDto>> Handle(ListChecklistItemsQuery request, CancellationToken cancellationToken)
    {
        var allItems = await _unitOfWork.Repository<MaintenanceChecklistItem>().ListAllAsync();
        
        var items = request.IncludeInactive 
            ? allItems 
            : allItems.Where(i => i.IsActive).ToList();
        
        return items
            .OrderBy(i => i.Order)
            .ThenBy(i => i.Title)
            .Select(c => new MaintenanceChecklistItemDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Order = c.Order,
                IsActive = c.IsActive
            }).ToList();
    }
}

