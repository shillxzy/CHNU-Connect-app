using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Group>> GetByCreatorIdAsync(int creatorId)
        {
            return await _dbSet
                .Where(g => g.CreatorId == creatorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetByTypeAsync(GroupType type)
        {
            return await _dbSet
                .Where(g => g.Type == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetWithCuratorByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbSet
                .Include(g => g.Curator)
                .Where(g => ids.Contains(g.Id))
                .ToListAsync();
        }
    }
}
