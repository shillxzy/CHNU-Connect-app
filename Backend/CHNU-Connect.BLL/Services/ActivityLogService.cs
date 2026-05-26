using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.BLL.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly AppDbContext _db;

        public ActivityLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(int userId, string userName, string action, string? entityType = null, int? entityId = null)
        {
            var entry = new ActivityLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow,
            };
            _db.ActivityLogs.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task<(IEnumerable<ActivityLogDto> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            DateTime? from = null, DateTime? to = null,
            string? action = null, int? userId = null)
        {
            var query = _db.ActivityLogs.AsQueryable();

            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);
            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(l => l.Action == action);
            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new ActivityLogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    UserName = l.UserName,
                    Action = l.Action,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Timestamp = l.Timestamp,
                })
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
