using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IAdminPermissionRepository : IGenericRepository<AdminPermission>
    {
        Task<IEnumerable<AdminPermission>> GetByUserIdAsync(int userId);
        Task<AdminPermission?> GetByUserAndTypeAsync(int userId, PermissionType type);
        Task<bool> HasPermissionAsync(int userId, PermissionType type);
    }
}
