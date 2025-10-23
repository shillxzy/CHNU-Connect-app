using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class AdminActionRepository : GenericRepository<AdminAction>, IAdminActionRepository
    {
        public AdminActionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AdminAction>> GetActionsByAdminIdAsync(int adminId)
        {
            return await _dbSet.Where(a => a.AdminId == adminId)
                              .OrderByDescending(a => a.ActionDate)
                              .ToListAsync();
        }

        public async Task<IEnumerable<AdminAction>> GetActionsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(a => a.UserId == userId)
                              .OrderByDescending(a => a.ActionDate)
                              .ToListAsync();
        }

        public async Task<IEnumerable<AdminAction>> GetRecentActionsAsync(int count)
        {
            return await _dbSet.OrderByDescending(a => a.ActionDate)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<IEnumerable<AdminAction>> GetActionsByTypeAsync(string actionType)
        {
            return await _dbSet.Where(a => a.ActionType == actionType)
                              .OrderByDescending(a => a.ActionDate)
                              .ToListAsync();
        }
    }
}
