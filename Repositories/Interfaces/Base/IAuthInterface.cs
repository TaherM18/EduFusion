namespace EduFusion.Repositories.Interfaces
{
    public interface IAuthInterface<T, K>
    {
        public Task<T> Login(K data);
        public Task<int> Register(T data);
    }
}