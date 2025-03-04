using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class TimeTable
    {
        public int? Id { get; set; }
        [Required]
        public TimeSpan  Start { get; set; }
        [Required]
        public TimeSpan  End { get; set; }
        [Required]
        public int DayOfWeek { get; set; }

        public int TeacherId { get; set; }

        public ClassModel? ClassModel { get; set; }
        public int ClassId { get; set; }
        public Teacher? Teacher { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int StandardId { get; set; }
        public Standard? Standard { get; set; }
    }
}