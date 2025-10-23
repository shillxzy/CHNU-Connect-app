using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IGroupMemberRepository : IGenericRepository<GroupMember>
    {
        Task<IEnumerable<GroupMember>> GetMembersByGroupIdAsync(int groupId);
        Task<IEnumerable<GroupMember>> GetGroupsByUserIdAsync(int userId);
        Task<bool> IsUserMemberOfGroupAsync(int userId, int groupId);
        Task<int> GetMembersCountByGroupIdAsync(int groupId);
        Task<GroupMember?> GetGroupMemberAsync(int userId, int groupId);
        Task<IEnumerable<GroupMember>> GetMembersByRoleAsync(int groupId, string role);
    }
}
