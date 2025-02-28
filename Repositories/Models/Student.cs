using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        public int StandardID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int RollNumber { get; set; }

        public string GuardianName { get; set; }

        [MaxLength(15)]
        public string GuardianContact { get; set; }

        [MaxLength(10)]
        public string Section { get; set; }

        [ForeignKey("StudentID")]
        public User User { get; set; }
    }
}