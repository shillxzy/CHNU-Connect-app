namespace CHNU_Connect.BLL.DTOs.AdminPermission
{
    public class AdminPermissionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = null!;
        public int GrantedByUserId { get; set; }
        public DateTime GrantedAt { get; set; }
    }
}
