using CHNU_Connect.BLL.DTOs.AdminPermission;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.BLL.Services
{
    public class AdminPermissionService : IAdminPermissionService
    {
        private readonly IAdminPermissionRepository _repo;

        public AdminPermissionService(IAdminPermissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AdminPermissionDto>> GetByUserIdAsync(int userId)
        {
            var perms = await _repo.GetByUserIdAsync(userId);
            return perms.Select(ToDto);
        }

        public async Task<AdminPermissionDto> GrantAsync(int userId, string type, int grantedByUserId)
        {
            if (!Enum.TryParse<PermissionType>(type, out var permType))
                throw new ArgumentException($"Unknown permission type: {type}");

            var existing = await _repo.GetByUserAndTypeAsync(userId, permType);
            if (existing != null)
                return ToDto(existing);

            var perm = new AdminPermission
            {
                UserId = userId,
                Type = permType,
                GrantedByUserId = grantedByUserId,
                GrantedAt = DateTime.UtcNow
            };

            await _repo.InsertAsync(perm);
            await _repo.SaveAsync();

            return ToDto(perm);
        }

        public async Task<bool> RevokeAsync(int userId, string type)
        {
            if (!Enum.TryParse<PermissionType>(type, out var permType))
                return false;

            var perm = await _repo.GetByUserAndTypeAsync(userId, permType);
            if (perm == null) return false;

            _repo.Delete(perm);
            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> HasPermissionAsync(int userId, string type)
        {
            if (!Enum.TryParse<PermissionType>(type, out var permType))
                return false;

            return await _repo.HasPermissionAsync(userId, permType);
        }

        public async Task<IEnumerable<string>> GetUserPermissionTypesAsync(int userId)
        {
            var perms = await _repo.GetByUserIdAsync(userId);
            return perms.Select(p => p.Type.ToString());
        }

        private static AdminPermissionDto ToDto(AdminPermission p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            Type = p.Type.ToString(),
            GrantedByUserId = p.GrantedByUserId,
            GrantedAt = p.GrantedAt
        };
    }
}
