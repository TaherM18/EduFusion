using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduFusion.Repositories.Models
{
    public class Material
    {
        [Key]
        public int MaterialID { get; set; }

        [Required, MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        public int UserID { get; set; }

        public int? SubjectID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("SubjectID")]
        public Subject Subject { get; set; }
    }
}