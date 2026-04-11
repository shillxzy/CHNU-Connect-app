using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class SubGroupRepository : GenericRepository<SubGroup>, ISubGroupRepository
    {
        public SubGroupRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SubGroup>> GetByGroupIdAsync(int groupId)
        {
            return await _dbSet
                .Where(sg => sg.GroupId == groupId)
                .ToListAsync();
        }
    }
}
