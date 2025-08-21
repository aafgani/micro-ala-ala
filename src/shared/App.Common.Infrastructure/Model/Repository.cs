using Microsoft.EntityFrameworkCore;

namespace App.Common.Infrastructure.Model
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _db;

        public Repository(DbContext db)
        {
            _db = db;
        }
        protected DbSet<T> Set => _db.Set<T>();

        protected IQueryable<T> Query(bool tracking = false)
        {
            return tracking ? _db.Set<T>() : _db.Set<T>().AsNoTracking();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);
            return entity;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _db.Set<T>().Update(entity);
            return await _db.SaveChangesAsync();
        }
    }
}
