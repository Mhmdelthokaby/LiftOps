using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Installation
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string message, Guid targetUserId);
        Task CreateNotificationAsync(LiftOps_BackEnd.Domain.Entities.Installation.Notification notification);
    }
}
