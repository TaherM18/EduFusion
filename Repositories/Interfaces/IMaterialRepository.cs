using EduFusion.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduFusion.interfaces
{

    public interface IMaterialRepository
    {
        Task<List<Material>> GetAllMaterialsAsync();
        Task<Material> GetMaterialByIdAsync(int id);
        Task<int> AddMaterialAsync(Material material);
        Task<bool> DeleteMaterialAsync(int id);
    }
}