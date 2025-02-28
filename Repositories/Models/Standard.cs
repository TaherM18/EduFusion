using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class Standard
    {
        [Key]
        public int StandardID { get; set; }

        [Required, MaxLength(255)]
        public string StandardName { get; set; }
    }

}