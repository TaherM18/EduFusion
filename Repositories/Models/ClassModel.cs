using System.ComponentModel.DataAnnotations;

namespace EduFusion.Repositories.Models
{
    public class ClassModel
    {
        [Key]
        public int ClassID { get; set; }

        [Required]
        [StringLength(255)]
        public string ClassName { get; set; }

        [StringLength(50)]
        public string? Wing { get; set; }  // Nullable since `c_wing` allows NULL in DB

        public int? Floor { get; set; }  // Nullable since `c_floor` allows NULL in DB
    }
}