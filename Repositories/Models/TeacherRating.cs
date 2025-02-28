using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class TeacherRating
    {
        [Key]
        public int RatingID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int TeacherID { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("StudentID")]
        public Student Student { get; set; }

        [ForeignKey("TeacherID")]
        public Teacher Teacher { get; set; }
    }
}