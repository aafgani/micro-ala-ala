namespace App.Common.Infrastructure.Model
{
    public interface IRepository<T>
    {
        IQueryable<T> Query(bool tracking = false);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task DeleteAsync(T album);
    }
}
