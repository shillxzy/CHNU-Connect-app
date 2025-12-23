using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<Chat?> GetChatWithMembersAsync(int chatId);
        Task<IEnumerable<Chat>> GetUserChatsAsync(int userId);
    }
}
