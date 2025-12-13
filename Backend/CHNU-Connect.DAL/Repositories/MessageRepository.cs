using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(int groupId)
        {
            return await _dbSet.Where(m => m.Id == groupId)
                              .OrderBy(m => m.SentAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(int userId)
        {
            return await _dbSet.Where(m => m.Id == userId)
                              .OrderByDescending(m => m.SentAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int groupId, int count)
        {
            return await _dbSet.Where(m => m.Id == groupId)
                              .OrderByDescending(m => m.SentAt)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<int> GetUnreadMessagesCountAsync(int userId, int groupId)
        {
            // Припускаємо, що є поле IsRead або подібне для відстеження прочитаних повідомлень
            return await _dbSet.CountAsync(m => m.Id == groupId && m.Id != userId);
        }
    }
}
