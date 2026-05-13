using CHNU_Connect.BLL.DTOs.Notification;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var list = await _repo.GetUnreadByUserIdAsync(userId);
            return list.Select(ToDto);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var list = await _repo.GetUnreadByUserIdAsync(userId);
            return list.Count();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            await _repo.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            await _repo.MarkAllAsReadAsync(userId);
        }

		public async Task<NotificationDto> CreateAsync(int userId, string type, int? entityId = null, int? actorId = null)
		{
			var entity = new Notification
			{
				UserId = userId,
				Type = type,
				EntityId = entityId,
				ActorId = actorId,
				IsRead = false,
				CreatedAt = DateTime.UtcNow,
			};
			await _repo.InsertAsync(entity);
			return ToDto(entity);
		}

		private static NotificationDto ToDto(Notification n) => new()
		{
			Id = n.Id,
			Type = n.Type,
			EntityId = n.EntityId,
			IsRead = n.IsRead,
			CreatedAt = n.CreatedAt,
			ActorId = n.ActorId,
			ActorName = n.Actor?.FullName,
			ActorAvatar = n.Actor?.PhotoUrl,
		};
	}
}
