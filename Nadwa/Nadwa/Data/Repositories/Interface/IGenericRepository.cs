using System.Linq.Expressions;

namespace Nadwa.Data.Repositories.Interface;

public interface IGenericRepository<T> where T : class {
    Task<T?> GetFirstOrDefaultAsync (Expression<Func<T, bool>>? predicate);

    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>? groupBy = null
    );

    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}