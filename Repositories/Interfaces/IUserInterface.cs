using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IUserInterface : IAuthInterface<User, LoginVM>
    {
        public Task<Student> GetStudent(User user, int sid);
        public Task<Teacher> GetTeacher(User user, int tid);
        public Task<User> GetOne(int id);

        public Task<List<User>> GetUsers(int adminId);
    }
}