using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class Notification
    {
        [Key]
        public int? NotificationID { get; set; }

        [Required]
        public string Message { get; set; } = "N/A";

        [Required]
        public int UserID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}