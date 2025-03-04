using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IMaterialInterface : IBaseInterface<Material>
    {
        public Task<List<Material>> GetAllByStandard(int standardID);
    }
}