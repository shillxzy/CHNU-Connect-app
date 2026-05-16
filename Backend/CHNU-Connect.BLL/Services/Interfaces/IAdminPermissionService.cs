using CHNU_Connect.BLL.DTOs.AdminPermission;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IAdminPermissionService
    {
        Task<IEnumerable<AdminPermissionDto>> GetByUserIdAsync(int userId);
        Task<AdminPermissionDto> GrantAsync(int userId, string type, int grantedByUserId);
        Task<bool> RevokeAsync(int userId, string type);
        Task<bool> HasPermissionAsync(int userId, string type);
        Task<IEnumerable<string>> GetUserPermissionTypesAsync(int userId);
    }
}
