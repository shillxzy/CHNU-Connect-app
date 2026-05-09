using System;

namespace CHNU_Connect.DAL.Entities
{
    public enum LessonType
    {
        Lecture,
        Practice
    }

    public enum WeekType
    {
        First = 1,
        Second = 2
    }

    public class Schedule
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? SubGroupId { get; set; } // null = лекція
        public SubGroup? SubGroup { get; set; }

        public LessonType Type { get; set; } // 🔥 ключове
        public WeekType Week { get; set; }   // 🔥 ключове

        public DayOfWeek Day { get; set; }

        public int SlotId { get; set; }
        public LessonSlot Slot { get; set; } = null!;

        // Модельки 
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string Location { get; set; }
    }

}
