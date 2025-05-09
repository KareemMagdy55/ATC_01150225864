using Nadwa.Data.Repositories.Interface;
using Nadwa.Models;
using Nadwa.Services.Caching;

namespace Nadwa.Data.Repositories.Implementation;

public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository {
    private readonly ApplicationDbContext _context;
    private readonly IRedisCacheService _redisCacheService;

    public ApplicationUserRepository(ApplicationDbContext context, IRedisCacheService redisCacheService) : base(context, redisCacheService) {
        _context = context;
        _redisCacheService = redisCacheService;
    }
    
}