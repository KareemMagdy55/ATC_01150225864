using Microsoft.EntityFrameworkCore;
using Nadwa.Data.Repositories.Interface;
using Nadwa.Models;
using Nadwa.Services.Caching;
using Nadwa.Utilites;

namespace Nadwa.Data.Repositories.Implementation;

public class EventRepository : GenericRepository<Event>, IEventRepository {
    private readonly ApplicationDbContext _context;
    private readonly IRedisCacheService _redisCacheService;

    public EventRepository(ApplicationDbContext context, IRedisCacheService redisCacheService) : base(context,
        redisCacheService) {
        _context = context;
        _redisCacheService = redisCacheService;
    }
    
}