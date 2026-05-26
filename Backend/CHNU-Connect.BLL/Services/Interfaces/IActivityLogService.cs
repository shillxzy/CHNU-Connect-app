namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogAsync(int userId, string userName, string action, string? entityType = null, int? entityId = null);

        Task<(IEnumerable<ActivityLogDto> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            DateTime? from = null, DateTime? to = null,
            string? action = null, int? userId = null);
    }

    public class ActivityLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
