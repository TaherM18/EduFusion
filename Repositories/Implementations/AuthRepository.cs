

using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class AuthRepository : IAuthInterface<User, LoginVM>
    {
        public async Task<User> Login(LoginVM vm)
        {
            return null;
        }
    }
}