using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "User email")]
        public string Email { get; set; } = "N/A";

        [Required]
        [Display(Name = "User password")]
        public string Password { get; set; } = "N/A";
    }
}