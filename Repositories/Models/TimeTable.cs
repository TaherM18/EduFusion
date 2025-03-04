namespace Repositories.Models
{
    public class TimeTable
    {
        public int TimetableID { get; set; }  // Maps to c_timetableID
        public int SubjectID { get; set; }    // Maps to c_subjectID
        public int ClassID { get; set; }      // Maps to c_classID
        public int DayOfWeek { get; set; }   // Maps to c_day_of_week
        public string? DayName { get; set; }
        public TimeSpan StartTime { get; set; }  // Maps to c_start_time
        public TimeSpan EndTime { get; set; }    // Maps to c_end_time

        // Navigation properties
        public Subject? Subject { get; set; }  // Represents relationship with t_subject
        public ClassModel? ClassModel { get; set; }      // Represents relationship with t_class

        public Standard? Standard { get; set; }
        public Teacher? Teacher { get; set; }
    }

}