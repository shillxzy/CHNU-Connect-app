using System;

namespace CHNU_Connect.DAL.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group? Group { get; set; }

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? SubGroupId { get; set; }
        public SubGroup? SubGroup { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string? Location { get; set; }
    }


}
