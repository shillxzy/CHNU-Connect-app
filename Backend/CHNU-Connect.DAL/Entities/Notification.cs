using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class Notification
    {
        public int Id { get; set; }  // PRIMARY KEY
        public int UserId { get; set; }  // для кого notification
        public string Type { get; set; } = null!;  // message / event / system
        public int? EntityId { get; set; }  // пов’язана подія: message_id / event_id / post_id
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // навігаційна властивість
        public User? User { get; set; }
    }
}
