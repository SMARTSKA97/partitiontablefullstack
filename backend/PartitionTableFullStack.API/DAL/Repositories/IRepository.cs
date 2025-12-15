using PartitionTableFullStack.API.Common;
using System.Linq.Expressions;

namespace PartitionTableFullStack.API.DAL.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PaginatedResponseObject<List<T>>> GetPagedAsync(QueryParameters queryParams);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> SaveChangesAsync();
    IQueryable<T> GetQueryable();
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
}
