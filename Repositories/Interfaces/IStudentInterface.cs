using EduFusion.Repositories.Models;

namespace EduFusion.Repositories.Interfaces
{
    public interface IStudentInterface : IBaseInterface<Student>
    {
        public Task<int> Register(Student student);
    }
}