using System;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Interfaces.Installation
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string message, Guid targetUserId);
        Task CreateNotificationAsync(Collins_BackEnd.Domain.Entities.Installation.Notification notification);
    }
}
