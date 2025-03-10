using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IMaterialInterface : IBaseInterface<Material>
    {
        public Task<List<Material>> GetAllByStandard(int standardID);
    }
}