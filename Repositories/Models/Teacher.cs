
using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Salary { get; set; }

        [Required, Range(0, 50)]
        public int ExperienceYears { get; set; }

        [Required, MaxLength(255)]
        public string Qualification { get; set; }

        [Required, MaxLength(255)]
        public string Expertise { get; set; }

        [ForeignKey("TeacherID")]
        public User User { get; set; }
    }
}