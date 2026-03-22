using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CHNU_Connect.DAL.Entities
{
    public enum UserRole
    {
        Student,
        Teacher,
        Admin,
        SuperAdmin
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public string? FullName { get; set; }
        public string? Faculty { get; set; }
        public int? Course { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsBlocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEmailConfirmed { get; set; } = true;
        public int? SubGroupId { get; set; }
        public SubGroup? SubGroup { get; set; }

        public string? EmailConfirmationToken { get; set; }

        // Для відновлення пароля
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }


        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Message>? SentMessages { get; set; }
        public ICollection<Message>? ReceivedMessages { get; set; }
        public ICollection<GroupMember>? Groups { get; set; }

    }
}
