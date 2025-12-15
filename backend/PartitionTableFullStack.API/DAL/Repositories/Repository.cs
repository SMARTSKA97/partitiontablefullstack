using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Common;
using PartitionTableFullStack.API.DAL.Extensions;
using PartitionTableFullStack.API.Data;
using System.Linq.Expressions;

namespace PartitionTableFullStack.API.DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<PaginatedResponseObject<List<T>>> GetPagedAsync(QueryParameters queryParams)
    {
        IQueryable<T> query = _dbSet;

        // Apply global search
        query = query.ApplyGlobalSearch(queryParams.GlobalSearch);

        // Apply filters
        query = query.ApplyFilters(queryParams.Filters);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (queryParams.Sorts != null && queryParams.Sorts.Any())
        {
            query = query.ApplySorts(queryParams.Sorts);
        }

        // Apply pagination
        var items = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return new PaginatedResponseObject<List<T>>
        {
            Data = items,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize
        };
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public virtual IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
}
