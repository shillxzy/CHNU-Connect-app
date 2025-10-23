using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId);
        Task<IEnumerable<Group>> GetPublicGroupsAsync();
        Task<Group?> GetGroupByNameAsync(string name);
        Task<int> GetGroupMembersCountAsync(int groupId);
        Task<bool> IsUserMemberOfGroupAsync(int groupId, int userId);
    }
}
