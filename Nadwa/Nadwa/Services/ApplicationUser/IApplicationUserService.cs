namespace Nadwa.Services.ApplicationUser;

public interface IApplicationUserService {
    Task<IEnumerable<Models.ApplicationUser>>? GetUsersPageAsync(int page = 1);
    Task<IEnumerable<Models.ApplicationUser>>? GetAllUsersAsync();
    Task<Models.ApplicationUser?> GetUserByIdAsync(string id);

    Task<string> UpdateUserBalance(Models.ApplicationUser? updatedApplicationUser);
    Task<string> AddUserAsync(Models.ApplicationUser? applicationUser);
    Task<string> DeleteUser(Models.ApplicationUser? applicationUser);
    Task<string> BookEventAsync(Models.ApplicationUser? applicationUser, Models.Event? e);
    Task<string> CancelEventAsync(Models.ApplicationUser? applicationUser, Models.Event? e);
    
}