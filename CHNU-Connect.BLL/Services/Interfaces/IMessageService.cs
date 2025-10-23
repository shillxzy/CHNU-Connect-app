using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IMessageService
    {
        Task<object> CreateMessageAsync(int senderId, int receiverId, string content);
        Task<object?> GetByIdAsync(int id);
        Task<IEnumerable<object>> GetConversationAsync(int userId1, int userId2);
        Task<IEnumerable<object>> GetUserMessagesAsync(int userId);
        Task<IEnumerable<object>> GetUnreadMessagesAsync(int userId);
        Task<bool> MarkAsReadAsync(int messageId);
        Task<bool> DeleteMessageAsync(int id);
    }
}
