using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IChatMemberRepository : IGenericRepository<ChatMember>
    {
        Task<ChatMember?> GetMemberAsync(int chatId, int userId);
        Task<List<ChatMember>> GetMembersByChatIdAsync(int chatId);

    }
}
