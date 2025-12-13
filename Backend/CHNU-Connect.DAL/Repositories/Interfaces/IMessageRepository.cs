using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(int groupId);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(int userId);
        Task<IEnumerable<Message>> GetRecentMessagesAsync(int groupId, int count);
        Task<int> GetUnreadMessagesCountAsync(int userId, int groupId);
    }
}
