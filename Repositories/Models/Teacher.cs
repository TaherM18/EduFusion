
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class Teacher
    {
        [Key]
        public int? TeacherID { get; set; }

        [Required, Range(1, float.MaxValue)]
        public float Salary { get; set; } = 10;

        [Required, Range(0, 50)]
        public int ExperienceYears { get; set; }

        [Required, MaxLength(255)]
        public string Qualification { get; set; } = "N/A";

        [Required, MaxLength(255)]
        public string Expertise { get; set; } = "N/A";

        [ForeignKey("TeacherID")]
        public User? User { get; set; }

        public bool IsApproved { get; set; }
    }
}