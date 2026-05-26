namespace CHNU_Connect.DAL.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
