using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduFusion.Repositories.Interfaces
{
    public interface ITeacherInterface : IBaseInterface<Teacher>
    {
        public Task<int> Register(Teacher teacher);
    }
}