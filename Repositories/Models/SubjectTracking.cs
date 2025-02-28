using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class SubjectTracking
    {
        [Key]
        public int TrackingID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Range(0, 100)]
        public decimal? Percentage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("SubjectID")]
        public Subject Subject { get; set; }
    }
}