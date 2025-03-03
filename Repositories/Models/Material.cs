using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class Material
    {
        [Key]
        public int? MaterialID { get; set; }

        [Required, MaxLength(255)]
        public string? FileName { get; set; }

        [Required]
        public int UserID { get; set; }

        public int? SubjectID { get; set; }

        public IFormFile? MaterialFile { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        [ForeignKey("SubjectID")]
        public Subject? Subject { get; set; }
    }
}