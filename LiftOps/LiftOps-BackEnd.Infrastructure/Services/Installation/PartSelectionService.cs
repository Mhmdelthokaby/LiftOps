using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Installation
{
    public class PartSelectionService : IPartSelectionService
    {
        private readonly IStageRequiredPartRepository _requiredPartRepository;
        private readonly IGenericRepository<InventoryItem> _inventoryRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public PartSelectionService(
            IStageRequiredPartRepository requiredPartRepository,
            IGenericRepository<InventoryItem> inventoryRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _requiredPartRepository = requiredPartRepository;
            _inventoryRepository = inventoryRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task AddPartsToStageAsync(Guid stageId, List<PartSelectionDto> parts)
        {
            foreach (var partDto in parts)
            {
                var item = await _inventoryRepository.GetByIdAsync(partDto.InventoryItemId);
                if (item == null) continue; // Or throw

                bool isOutOfStock = item.StockQuantity < partDto.Quantity; // Assuming Quantity is stock

                var requiredPart = new StageRequiredPart
                {
                    StageId = stageId,
                    InventoryItemId = partDto.InventoryItemId,
                    Quantity = partDto.Quantity,
                    IsOutOfStock = isOutOfStock
                };

                _requiredPartRepository.Add(requiredPart);

                if (isOutOfStock)
                {
                    // TargetUserId: InventoryAdmin. We'd fetch this from Roles/Users. 
                    // For now using a placeholder or we need a service to find InventoryAdmin ID.
                    // Simplification: We'll skip finding the actual User ID in this turn and use Guid.Empty or a TODO.
                    // Better: NotificationService handles finding the user or we broadcast to Role?
                    // The Entity expects TargetUserId. 
                    // I'll log a TODO: "Find Admin Id".
                    // Or I can query Identity if I had UserManager here.
                    
                    // Actually, let's just make the message specific.
                    await _notificationService.CreateNotificationAsync(
                        $"Part {item.Name} is Out of Stock for Stage {stageId}", 
                        Guid.Empty // Pending: Resolve Admin ID
                    );
                }
            }
            await _unitOfWork.Complete();
        }
    }
}
