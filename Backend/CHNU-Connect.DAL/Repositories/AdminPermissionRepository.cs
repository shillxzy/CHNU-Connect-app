using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class AdminPermissionRepository : GenericRepository<AdminPermission>, IAdminPermissionRepository
    {
        public AdminPermissionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<AdminPermission>> GetByUserIdAsync(int userId)
        {
            return await _dbSet.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<AdminPermission?> GetByUserAndTypeAsync(int userId, PermissionType type)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);
        }

        public async Task<bool> HasPermissionAsync(int userId, PermissionType type)
        {
            return await _dbSet.AnyAsync(p => p.UserId == userId && p.Type == type);
        }
    }
}
