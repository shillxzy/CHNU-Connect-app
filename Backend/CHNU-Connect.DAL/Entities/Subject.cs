using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHNU_Connect.DAL.Entities
{
    public class Subject
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        public int GroupId { get; set; }
        public Group? Group { get; set; }

        public int? TeacherId { get; set; }
        public User? Teacher { get; set; }

        public string? MoodleLink { get; set; }

        public int Semester { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Schedule>? Schedules { get; set; }
    }

}
