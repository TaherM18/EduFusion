using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IStudentInterface : IBaseInterface<Student>
    {
        public Task<int> Register(Student student);

        public Task<int> Approve(int sid);
    }
}