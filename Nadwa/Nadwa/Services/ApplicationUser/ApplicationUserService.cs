using Nadwa.Data.Repositories.Interface;
using Nadwa.Utilites;

namespace Nadwa.Services.ApplicationUser;

public class ApplicationUserService : IApplicationUserService {
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationUserService(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Models.ApplicationUser>>? GetUsersPageAsync(int page = 1) {
        return await _unitOfWork
            .ApplicationUserRepository
            .GetPagedAsync(page);
    }


    public async Task<IEnumerable<Models.ApplicationUser>>? GetAllUsersAsync() {
        return await _unitOfWork
            .ApplicationUserRepository
            .GetAllAsync();
    }

    public async Task<object> GetUserByIdAsync(string id) {
        var res = await _unitOfWork
            .ApplicationUserRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == id);
        if (res is null)
            return Messages.Fail.UserDelete;

        return res;
    }


    public async Task<string> AddUserAsync(Models.ApplicationUser? applicationUser) {
        if (applicationUser is null)
            return Messages.Fail.UserAdd;

        await _unitOfWork
            .ApplicationUserRepository
            .AddAsync(applicationUser);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.UserAdd;
    }

    public async Task<string> DeleteUser(Models.ApplicationUser? applicationUser) {
        if (applicationUser is null)
            return Messages.Fail.UserDelete;

        _unitOfWork
            .ApplicationUserRepository
            .Remove(applicationUser);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.UserDelete;
    }

    public async Task<string> BookEvent(Models.ApplicationUser? applicationUser, Models.Event? e) {
        if (applicationUser is null || e is null)
            return Messages.Fail.BookingEvent;

        if (e.Price > applicationUser.Balance)
            return Messages.Fail.BookingHighCostEvent;


        applicationUser.Events.Add(e);
        applicationUser.Balance -= e.Price;

        e.Attendees.Add(applicationUser);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.BookingEvent;
    }

    public async Task<string> CancelEvent(Models.ApplicationUser? applicationUser, Models.Event? e) {
        if (applicationUser is null || e is null)
            return Messages.Fail.CancelEvent;

        applicationUser.Events.Remove(e);
        applicationUser.Balance += e.Price;

        e.Attendees.Remove(applicationUser);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.BookingEvent;
    }
}