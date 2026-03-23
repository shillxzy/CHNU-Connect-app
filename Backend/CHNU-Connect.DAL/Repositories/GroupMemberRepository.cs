using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class GroupMemberRepository : GenericRepository<GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<GroupMember?> GetAsync(int groupId, int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);
        }

        public async Task<bool> IsMemberAsync(int groupId, int userId)
        {
            return await _dbSet
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);
        }

        public async Task<IEnumerable<GroupMember>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupMember>> GetByGroupIdAsync(int groupId)
        {
            return await _context.GroupMembers
                .Include(m => m.User) 
                .Where(m => m.GroupId == groupId)
                .ToListAsync();
        }

    }
}
