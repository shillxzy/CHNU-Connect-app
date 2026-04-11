using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface ISubGroupRepository : IGenericRepository<SubGroup>
    {
        Task<IEnumerable<SubGroup>> GetByGroupIdAsync(int groupId);
    }
}
