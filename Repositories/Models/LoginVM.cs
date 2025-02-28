using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EduFusion.Repositories.Models
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "User email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User password")]
        public string Password { get; set; }
    }
}