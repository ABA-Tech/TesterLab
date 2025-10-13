using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TesterLab.Domain.interfaces.Repositories;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TesterLabDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(TesterLabDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            // Gérer CreatedAt et UpdatedAt si l'entité les possède
            if (entity.GetType().GetProperty("CreatedAt") != null)
            {
                entity.GetType().GetProperty("CreatedAt")!.SetValue(entity, DateTime.UtcNow);
            }

            if (entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entity.GetType().GetProperty("UpdatedAt")!.SetValue(entity, DateTime.UtcNow);
            }

            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // Gérer UpdatedAt si l'entité le possède
            if (entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entity.GetType().GetProperty("UpdatedAt")!.SetValue(entity, DateTime.UtcNow);
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();
            var items = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
