using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nadwa.Data.Repositories.Interface;
using Nadwa.Models;
using Nadwa.Services.Caching;

namespace Nadwa.Data.Repositories.Implementation;

public class UnitOfWork : IUnitOfWork {
    private readonly ApplicationDbContext _context;
    private readonly IRedisCacheService _cache;
    
    public IEventRepository EventRepository { get; private set; }
    public IApplicationUserRepository ApplicationUserRepository { get; private set; }

    public UnitOfWork(ApplicationDbContext context, IRedisCacheService redisCacheService) {
        _context = context;
        _cache = redisCacheService;

        EventRepository = new EventRepository(context, _cache);
        ApplicationUserRepository = new ApplicationUserRepository(context, _cache);
    }


    public void Dispose() {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync() {
        await _context.DisposeAsync();
    }

    public async Task<int> CompleteAsync() {
        return await _context.SaveChangesAsync();
    }
    
  
}