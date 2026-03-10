using CHNU_Connect.DAL.Entities;


namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task InsertAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
    }
}
