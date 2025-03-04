using Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{

    public interface IMaterialInterface
    {
        Task<List<Material>> GetAllMaterialsAsync();
        Task<Material> GetMaterialByIdAsync(int id);
        Task<int> AddMaterialAsync(Material material);
        Task<bool> DeleteMaterialAsync(int id);
    }
}