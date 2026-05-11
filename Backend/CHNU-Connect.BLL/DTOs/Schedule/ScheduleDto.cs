using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.BLL.DTOs.Schedule
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int? SubGroupId { get; set; }
        public string? SubGroupName { get; set; }
        public LessonType Type { get; set; }
        public WeekType Week { get; set; }
        public bool IsEveryWeek { get; set; } // ✅ NEW
        public DayOfWeek Day { get; set; }
        public int SlotId { get; set; }
        public int PairNumber { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string? Location { get; set; }
    }
}
