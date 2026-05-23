using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CreatorId { get; set; }
        public User? Creator { get; set; }

        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }

        public ICollection<EventParticipant>? Participants { get; set; }
    }
}
