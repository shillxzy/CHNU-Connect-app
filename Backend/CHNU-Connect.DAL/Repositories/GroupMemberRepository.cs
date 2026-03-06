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

        public async Task<IEnumerable<GroupMember>> GetMembersByGroupIdAsync(int groupId)
        {
            return await _dbSet.Where(gm => gm.GroupId == groupId)
                              .ToListAsync();
        }

        public async Task<IEnumerable<GroupMember>> GetGroupsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(gm => gm.UserId == userId)
                              .ToListAsync();
        }

        public async Task<bool> IsUserMemberOfGroupAsync(int userId, int groupId)
        {
            return await _dbSet.AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        }

        public async Task<int> GetMembersCountByGroupIdAsync(int groupId)
        {
            return await _dbSet.CountAsync(gm => gm.GroupId == groupId);
        }

        public async Task<GroupMember?> GetGroupMemberAsync(int userId, int groupId)
        {
            return await _dbSet.FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        }

        public async Task<IEnumerable<GroupMember>> GetMembersByRoleAsync(int groupId, string role)
        {
            return await _dbSet.Where(gm => gm.GroupId == groupId && gm.Role == role)
                              .ToListAsync();
        }
    }
}
