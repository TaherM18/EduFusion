using EduFusion.Repositories.Models;

namespace EduFusion.Repositories.Interfaces
{
    public interface IUserInterface : IAuthInterface<User, LoginVM,Teacher>;
}