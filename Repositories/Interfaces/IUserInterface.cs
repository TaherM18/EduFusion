using Repositories.Models;

namespace Repositories.Interfaces{
    public interface IUserInterface : IAuthInterface<User, LoginVM>;
}