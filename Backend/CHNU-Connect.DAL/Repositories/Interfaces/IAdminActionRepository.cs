using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IAdminActionRepository : IGenericRepository<AdminAction>
    {
        Task<IEnumerable<AdminAction>> GetActionsByAdminIdAsync(int adminId);
        Task<IEnumerable<AdminAction>> GetActionsByUserIdAsync(int userId);
        Task<IEnumerable<AdminAction>> GetRecentActionsAsync(int count);
        Task<IEnumerable<AdminAction>> GetActionsByTypeAsync(string actionType);
    }
}
