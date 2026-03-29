using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;
using System.Linq;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(Guid Id, UpdateMaintenanceChecklistItemDto Dto) : IRequest<bool>;

public class UpdateChecklistItemCommandHandler : IRequestHandler<UpdateChecklistItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateChecklistItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var checklistRepo = _unitOfWork.Repository<Domain.Entities.Maintenance.MaintenanceChecklistItem>();
        var checklistItem = await checklistRepo.GetByIdAsync(request.Id);

        if (checklistItem == null)
        {
            return false;
        }

        int oldOrder = checklistItem.Order;
        int newOrder = request.Dto.Order;

        // If order is changing, shift other items
        if (oldOrder != newOrder)
        {
            var allItems = await checklistRepo.ListAllAsync();
            var orderedItems = allItems.OrderBy(x => x.Order).ToList();

            if (newOrder > oldOrder)
            {
                // Moving down: shift items between oldOrder and newOrder up by 1
                var itemsToShift = orderedItems
                    .Where(x => x.Id != request.Id && x.Order > oldOrder && x.Order <= newOrder)
                    .ToList();
                foreach (var item in itemsToShift)
                {
                    item.Order -= 1;
                    checklistRepo.Update(item);
                }
            }
            else
            {
                // Moving up: shift items between newOrder and oldOrder down by 1
                var itemsToShift = orderedItems
                    .Where(x => x.Id != request.Id && x.Order >= newOrder && x.Order < oldOrder)
                    .ToList();
                foreach (var item in itemsToShift)
                {
                    item.Order += 1;
                    checklistRepo.Update(item);
                }
            }
        }

        checklistItem.Title = request.Dto.Title;
        checklistItem.Description = request.Dto.Description;
        checklistItem.Order = newOrder;
        checklistItem.IsActive = request.Dto.IsActive;

        checklistRepo.Update(checklistItem);
        var result = await _unitOfWork.Complete();

        return result > 0;
    }
}

