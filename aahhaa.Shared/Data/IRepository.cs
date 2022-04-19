using System.Linq.Expressions;

namespace aahhaa.Shared.Data.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetAsync(object key);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object key);
    Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
