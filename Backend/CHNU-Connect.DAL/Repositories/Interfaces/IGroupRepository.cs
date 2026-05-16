using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<IEnumerable<Group>> GetByCreatorIdAsync(int creatorId);
        Task<IEnumerable<Group>> GetByTypeAsync(GroupType type);
        Task<IEnumerable<Group>> GetWithCuratorByIdsAsync(IEnumerable<int> ids);
    }
}
