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

        /*
         * Returning IQueryable<T> gives consumers (e.g., service layer) the flexibility 
         * to filter, sort, paginate, or project before the query is sent to the database.
         */
        public IQueryable<T> Query(bool tracking = false)
        {
            return tracking ? _db.Set<T>() : _db.Set<T>().AsNoTracking();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T album)
        {
            _db.Set<T>().Remove(album);
            await _db.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);
            return entity ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _db.Set<T>().Update(entity);
            return await _db.SaveChangesAsync();
        }
    }
}
