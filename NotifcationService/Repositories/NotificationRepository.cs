using Infrastructure;
using Models.Models;
using NotifcationService.Interfaces.Repositories;

namespace NotifcationService.Repositories
{
    public class NotificationRepository : RepositoryBase<Notification, NotificationContext>, INotificationRepository
    {
        public NotificationRepository(NotificationContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}