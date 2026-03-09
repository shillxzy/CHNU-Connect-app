namespace CHNU_Connect.BLL.DTOs.User
{
    public class PublicUserProfileDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Faculty { get; set; }
        public int? Course { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
