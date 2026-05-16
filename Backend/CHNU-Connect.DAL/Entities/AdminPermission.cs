namespace CHNU_Connect.DAL.Entities
{
    public enum PermissionType
    {
        ManageContent,   // posts, comments
        ManageSchedule,  // timetable editing
        ManageUsers,     // block/unblock, role changes
        ManageEvents     // events, news feed
    }

    public class AdminPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public PermissionType Type { get; set; }
        public int GrantedByUserId { get; set; }
        public User? GrantedBy { get; set; }
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    }
}
