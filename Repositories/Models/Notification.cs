using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduFusion.Repositories.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}