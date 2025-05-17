using System.Linq.Expressions;

namespace Nadwa.Data.Repositories.Interface;

public interface IGenericRepository<T> where T : class {
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate, IEnumerable<T>? enumerable = null);

    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>? groupBy = null, IEnumerable<T>? enumerable = null
    );

    Task<IEnumerable<T>> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? groupBy = null
        , IEnumerable<T>? enumerable = null);


    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}