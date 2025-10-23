using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
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
            return await _dbSet.Where(m => m.GroupId == groupId)
                              .OrderBy(m => m.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(int userId)
        {
            return await _dbSet.Where(m => m.UserId == userId)
                              .OrderByDescending(m => m.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int groupId, int count)
        {
            return await _dbSet.Where(m => m.GroupId == groupId)
                              .OrderByDescending(m => m.CreatedAt)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<int> GetUnreadMessagesCountAsync(int userId, int groupId)
        {
            // Припускаємо, що є поле IsRead або подібне для відстеження прочитаних повідомлень
            return await _dbSet.CountAsync(m => m.GroupId == groupId && m.UserId != userId);
        }
    }
}
