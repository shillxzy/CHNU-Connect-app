using CHNU_Connect.DAL.Entities;


namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task InsertAsync(Notification notification);
    }
}
