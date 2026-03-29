using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Services.Installation
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork; // Assuming UnitOfWork is used for saves

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateNotificationAsync(string message, Guid targetUserId)
        {
            var notification = new Notification
            {
                Message = message,
                TargetUserId = targetUserId,
                IsRead = false
                // Title and Type will be default/empty for this old method
            };
            
            _notificationRepository.Add(notification);
            await _unitOfWork.Complete();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            _notificationRepository.Add(notification);
            await _unitOfWork.Complete();
        }
    }
}
