using Collins_BackEnd.Domain.Entities.Installation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Domain.Interfaces.Installation
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(Guid userId);
    }
}
