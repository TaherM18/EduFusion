using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class Student
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

        [ForeignKey("StudentID")]
        public User? User { get; set; }
        public Standard? Standard { get; set; }
    }
}