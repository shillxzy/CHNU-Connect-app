using CHNU_Connect.BLL.DTOs.Notification;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);                          // NEW
		Task<NotificationDto> CreateAsync(int userId, string type, int? entityId = null, int? actorId = null, string? body = null);
		Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync(int userId);
	}
}
