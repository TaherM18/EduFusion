using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IExamInterface : IBaseInterface<Exam>
    {
        public Task<List<Exam>> GetAllByStandard(int standardID);
    }
}