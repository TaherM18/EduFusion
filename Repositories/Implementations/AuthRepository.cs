using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class AuthRepository : IAuthInterface<User, LoginVM>
    {
        public async Task<Student> Login(LoginVM vm)
        {
            return null;
        }
    }
}