using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IGroupMemberRepository : IGenericRepository<GroupMember>
    {
        Task<GroupMember?> GetAsync(int groupId, int userId);
        Task<bool> IsMemberAsync(int groupId, int userId);
        Task<IEnumerable<GroupMember>> GetByUserIdAsync(int userId);
        Task<IEnumerable<GroupMember>> GetByGroupIdAsync(int groupId);
    }
}
