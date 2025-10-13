using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TesterLab.Domain.interfaces.Services
{
  public interface IGenericService<T> where T : class
  {
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<bool> ValidateAsync(T entity);
  }
}
