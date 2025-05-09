using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nadwa.Data.Repositories.Interface;
using Nadwa.Services.Caching;
using Nadwa.Utilites;
using NuGet.Protocol;

namespace Nadwa.Data.Repositories.Implementation;

public class GenericRepository<T> : IGenericRepository<T> where T : class {
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly IRedisCacheService _redisCacheService;

    public GenericRepository(ApplicationDbContext context, IRedisCacheService redisCacheService) {
        _context = context;
        _dbSet = context.Set<T>();
        _redisCacheService = redisCacheService;
    }


    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null) {
        IQueryable<T> query = _dbSet;
        var cacheKey = $"{typeof(T).Name} " +
                       $"{predicate?.Body.ToString()}_";

        var cached = _redisCacheService.GetData<T>(cacheKey);

        if (cached is not null) return cached;


        if (predicate is not null) {
            query = query.Where(predicate);
        }

        var result = await query.FirstOrDefaultAsync();
        _redisCacheService.SetData(cacheKey, result);

        return result;
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>? groupBy = null) {
        var cacheKey = $"{typeof(T).Name}_All_" +
                       $"{predicate?.Body.ToString()}_" +
                       $"{orderBy?.Method.Name}_" +
                       $"{groupBy?.Body.ToString()}";

        var cached = _redisCacheService.GetData<IEnumerable<T>>(cacheKey);

        if (cached is not null) return cached;

        IQueryable<T> query = _context.Set<T>();

        if (predicate != null)
            query = query.Where(predicate);

        if (groupBy != null)
            query = query.GroupBy(groupBy).SelectMany(g => g);

        if (orderBy != null)
            query = orderBy(query);

        var result = await query.ToListAsync();
        _redisCacheService.SetData(cacheKey, result);

        return result;
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page = 1, int pageSize = 4,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? groupBy = null) {
        var cacheKey = $"{typeof(T).Name}_All_" +
                       $"{page}" +
                       $"{predicate?.Body.ToString()}_" +
                       $"{orderBy?.Method.Name}_" +
                       $"{groupBy?.Body.ToString()}";

        var cached = _redisCacheService.GetData<IEnumerable<T>>(cacheKey);

        if (cached is not null) return cached;

        IQueryable<T> query = _context.Set<T>();

        if (predicate is not null)
            query = query.Where(predicate);

        query = orderBy is not null ? orderBy(query) : query.OrderBy(e => 0);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _redisCacheService.SetData(cacheKey, result);

        return result;
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(T entity) => _dbSet.Remove(entity);
}