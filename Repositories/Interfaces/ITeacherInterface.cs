using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITeacherInterface : IBaseInterface<Teacher>
    {
        public Task<int> Register(Teacher teacher);
    }
}