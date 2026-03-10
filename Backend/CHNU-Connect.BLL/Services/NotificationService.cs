using CHNU_Connect.BLL.DTOs.Notification;
using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.BLL.Services.Interfaces
{

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                EntityId = n.EntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            return notifications.Count();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
        }
    }
}
