using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class StudentProgress
    {
        [Key]
        public int TrackingID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Percentage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; }
    }
}
