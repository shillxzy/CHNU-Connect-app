using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Schedule>> GetByGroupIdAsync(int groupId)
        {
            return await _dbSet
                .Include(s => s.Subject)
                .Where(s => s.GroupId == groupId)
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetBySubGroupIdAsync(int subGroupId)
        {
            return await _dbSet
                .Include(s => s.Subject)
                .Where(s => s.SubGroupId == subGroupId)
                .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
