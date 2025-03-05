using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class StudentViewModel
    {
        [Key]
        public int? StudentID { get; set; }

        [Required]
        public int StandardID { get; set; }

        [Required]
        public string RollNumber { get; set; } = "0";

        public string GuardianName { get; set; } = "N/A";

        [MaxLength(15)]
        public string GuardianContact { get; set; } = "0000000000";

        [MaxLength(10)]
        public string Section { get; set; } = "N/A";

        public bool IsApproved { get; set; }

        // 

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(255)]
        public string LastName { get; set; } = "";

        [Required]
        public DateTime? BirthDate { get; set; }

        [Required, MaxLength(255)]
        public string Gender { get; set; } = "";

        public string? Image { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required, MaxLength(15)]
        public string Contact { get; set; } = "0000000000";


        [Required, MaxLength(1)]
        public string Role { get; set; } = "S"; // "A"=Admin, "S"=Student, "T"=Teacher

        public string Address { get; set; } = "";

        [MaxLength(6)]
        public string Pincode { get; set; } = "000000";

        public bool IsActive { get; set; } = true;

        public IFormFile? ImageFile { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}