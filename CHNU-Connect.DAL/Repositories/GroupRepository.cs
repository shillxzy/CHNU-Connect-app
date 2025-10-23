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

        public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(g => g.Members.Any(gm => gm.UserId == userId))
                              .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetPublicGroupsAsync()
        {
            return await _dbSet.Where(g => g.IsPublic)
                              .OrderBy(g => g.Name)
                              .ToListAsync();
        }

        public async Task<Group?> GetGroupByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(g => g.Name == name);
        }

        public async Task<int> GetGroupMembersCountAsync(int groupId)
        {
            return await _context.GroupMembers.CountAsync(gm => gm.GroupId == groupId);
        }

        public async Task<bool> IsUserMemberOfGroupAsync(int groupId, int userId)
        {
            return await _context.GroupMembers.AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
        }
    }
}
