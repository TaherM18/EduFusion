using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }

        [MaxLength(255)]
        public string SubjectName { get; set; }

        [Range(0, 100)]
        public decimal? Marks { get; set; } = 100;

        public int? StandardID { get; set; }
        public int? TeacherID { get; set; }

        [ForeignKey("StandardID")]
        public Standard Standard { get; set; }

        [ForeignKey("TeacherID")]
        public Teacher Teacher { get; set; }
    }
}