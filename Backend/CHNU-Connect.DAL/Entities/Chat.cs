using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } // "private" або "group"

        [MaxLength(255)]
        public string? Title { get; set; } // null для приватних чатів

        [Required]
        public int CreatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public User Creator { get; set; }

        public string? DirectKey { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatMember> Members { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
