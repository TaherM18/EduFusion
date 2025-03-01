using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class Exam
    {
        [Key]
        public int? ExamID { get; set; }

        [MaxLength(255)]
        public string ExamName { get; set; } = "N/A";

        [Required]
        public int SubjectID { get; set; }

        [Required]
        public int ClassID { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required, Range(0, 500)]
        public float Duration { get; set; } = 1.0f;

        [ForeignKey("SubjectID")]
        public Subject? Subject { get; set; }
    }
}