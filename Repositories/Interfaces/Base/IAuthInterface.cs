namespace Repositories.Interfaces
{
    public interface IAuthInterface<T, K>
    {
        public Task<T> Login(K data);
    }
}