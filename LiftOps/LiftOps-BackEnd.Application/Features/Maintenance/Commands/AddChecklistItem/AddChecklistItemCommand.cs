using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.AddChecklistItem;

public record AddChecklistItemCommand(CreateMaintenanceChecklistItemDto Dto) : IRequest<Guid?>;

public class AddChecklistItemCommandHandler : IRequestHandler<AddChecklistItemCommand, Guid?>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddChecklistItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid?> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var checklistRepo = _unitOfWork.Repository<MaintenanceChecklistItem>();
        
        // Get all existing checklist items ordered by Order
        var allItems = await checklistRepo.ListAllAsync();
        var orderedItems = allItems.OrderBy(x => x.Order).ToList();
        
        // Find the target order position
        int targetOrder = request.Dto.Order;
        
        // Shift all items with order >= targetOrder by incrementing their order by 1
        // This ensures no two items have the same order
        var itemsToShift = orderedItems.Where(x => x.Order >= targetOrder).ToList();
        foreach (var item in itemsToShift)
        {
            item.Order += 1;
            checklistRepo.Update(item);
        }

        var checklistItem = new MaintenanceChecklistItem
        {
            Title = request.Dto.Title,
            Description = request.Dto.Description,
            Order = targetOrder,
            IsActive = true
        };

        checklistRepo.Add(checklistItem);
        var result = await _unitOfWork.Complete();

        return result > 0 ? checklistItem.Id : null;
    }
}

