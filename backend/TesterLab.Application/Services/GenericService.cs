using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;

namespace TesterLab.Applications.Services
{
  public class GenericService<T> : IGenericService<T> where T : class
  {
    protected readonly IGenericRepository<T> _repository;

    public GenericService(IGenericRepository<T> repository)
    {
      _repository = repository;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _repository.GetAllAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
      return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
      return await _repository.FindAsync(predicate);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
      await ValidateAsync(entity);
      return await _repository.CreateAsync(entity);
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
      await ValidateAsync(entity);
      return await _repository.UpdateAsync(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
      var entity = await _repository.GetByIdAsync(id);
      if (entity == null)
        throw new ArgumentException($"Entity with id {id} not found");

      return await _repository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
      return await _repository.ExistsAsync(id);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
      if (page <= 0) page = 1;
      if (pageSize <= 0) pageSize = 10;
      if (pageSize > 100) pageSize = 100; // Limite max

      return await _repository.GetPagedAsync(page, pageSize);
    }

    public virtual async Task<bool> ValidateAsync(T entity)
    {
      // Validation de base - peut être override dans les services spécialisés
      if (entity == null)
        throw new ArgumentNullException(nameof(entity));

      return await Task.FromResult(true);
    }
  }
}
