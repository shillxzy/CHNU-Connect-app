using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Repositories
{
    public class ChatMemberRepository : GenericRepository<ChatMember>, IChatMemberRepository
    {
        public ChatMemberRepository(AppDbContext context) : base(context) { }

        public async Task<ChatMember?> GetMemberAsync(int chatId, int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.ChatId == chatId && m.UserId == userId);
        }
    }
}
