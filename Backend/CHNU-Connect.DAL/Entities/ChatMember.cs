using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class ChatMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChatId { get; set; }
        [ForeignKey(nameof(ChatId))]
        public Chat Chat { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "member"; // owner, admin, member

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public int? LastReadMessageId { get; set; }
    }
}
