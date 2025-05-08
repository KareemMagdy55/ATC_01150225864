using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nadwa.Data.Repositories.Interface;

namespace Nadwa.Data.Repositories.Implementation;

public class GenericRepository<T> : IGenericRepository<T> where T : class {
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context) {
        _context = context;
        _dbSet = context.Set<T>();
    }


    public async Task<T?> GetByIdAsync(string id, Expression<Func<T, bool>>? predicate = null) {
        IQueryable<T> query = _dbSet;
        if (predicate is not null) {
            query = query.Where(predicate);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>? groupBy = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (predicate != null)
            query = query.Where(predicate);

        if (groupBy != null)
            query = query.GroupBy(groupBy).SelectMany(g => g);

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync();
    }
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(T entity) => _dbSet.Remove(entity);
}