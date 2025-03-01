using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class User
    {
        [Key]
        public int? UserID { get; set; }

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(255)]
        public string LastName { get; set; } = "";

        [Required]
        public DateTime BirthDate { get; set; }

        [Required, MaxLength(255)]
        public string Gender { get; set; } = "";

        public string? Image { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required, MaxLength(15)]
        public string Contact { get; set; } = "0000000000";

        public bool Status { get; set; } = true;

        [Required, MaxLength(1)]
        public string Role { get; set; } = "S"; // "A"=Admin, "S"=Student, "T"=Teacher

        public string Address { get; set; } = "";

        [MaxLength(6)]
        public string Pincode { get; set; } = "000000";

        public IFormFile? ImageFile { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}