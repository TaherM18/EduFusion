namespace Repositories.Interfaces
{
    public interface IBaseInterface<T>
    {
        public Task<T> GetOne(int id);

        public Task<List<T>> GetAll();

        public Task<int> Add(T data);
        
        public Task<int> Update(T data);

        public Task<int> Delete(int id);
    }
}