using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IUserInterface : IAuthInterface<User, LoginVM>
    {
        public Task<Student> GetStudent(User user, int sid);
        public Task<Teacher> GetTeacher(User user, int tid);
    }
}