using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class Standard
    {
        [Key]
        public int? StandardID { get; set; }

        [Required, MaxLength(255)]
        public string StandardName { get; set; } = "N/A";

        public List<Subject>? Subjects { get; set; }
    }

}