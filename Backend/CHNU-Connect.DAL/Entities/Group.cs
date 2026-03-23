using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public enum GroupType
    {
        Academic,
        Course,
        Announcement
    }

    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int CreatorId { get; set; }
        public User? Creator { get; set; }

        public bool IsPrivate { get; set; } = true;


        public int? CuratorId { get; set; }
        public User? Curator { get; set; }

        public GroupType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GroupMember>? Members { get; set; }
        public ICollection<Subject>? Subjects { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
    }

}
