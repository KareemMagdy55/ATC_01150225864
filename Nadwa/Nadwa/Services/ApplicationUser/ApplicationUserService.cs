using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Nadwa.Data.Repositories.Interface;
using Nadwa.Utilites;
using NuGet.Protocol.Plugins;

namespace Nadwa.Services.ApplicationUser;

public class ApplicationUserService : IApplicationUserService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITempDataProvider _tempDataProvider;

    public ApplicationUserService(IUnitOfWork unitOfWork, ITempDataProvider tempDataProvider) {
        _unitOfWork = unitOfWork;
        _tempDataProvider = tempDataProvider;
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

    public async Task<Models.ApplicationUser?> GetUserByIdAsync(string id) {
        var res = await _unitOfWork
            .ApplicationUserRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == id);

        return res;
    }

    public async Task<string> UpdateUserBalance(Models.ApplicationUser? updatedApplicationUser) {
        var user = await _unitOfWork
            .ApplicationUserRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == updatedApplicationUser.Id);

        if (user is null) return Messages.Fail.BalanceUpdate;
        user.Balance = updatedApplicationUser.Balance;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ApplicationUserRepository.Update(user);
        await _unitOfWork.CompleteAsync();
        return Messages.Success.BalanceUpdate;
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

    public async Task<string> BookEventAsync(Models.ApplicationUser? applicationUser, Models.Event? e) {
        if (applicationUser is null || e is null)
            return Messages.Fail.BookingEvent;

        if (e.Price > applicationUser.Balance)
            return Messages.Fail.BookingHighCostEvent;
        if (applicationUser.Events.Contains(e))
            return Messages.Fail.EventAlreadyExist;

        applicationUser.Events.Add(e);
        applicationUser.Balance -= e.Price;

        e.Attendees.Add(applicationUser);
        e.UpdatedAt = DateTime.UtcNow;
        applicationUser.UpdatedAt = DateTime.UtcNow;


        _unitOfWork.EventRepository.Update(e);
        _unitOfWork.ApplicationUserRepository.Update(applicationUser);
        await _unitOfWork.CompleteAsync();
        return Messages.Success.BookingEvent;
    }

    public async Task<string> CancelEventAsync(Models.ApplicationUser? applicationUser, Models.Event? e) {
        if (applicationUser is null || e is null)
            return Messages.Fail.CancelEvent;
        if (!applicationUser.Events.Contains(e))
            return Messages.Fail.EventNotExist;
        

        applicationUser.Events.Remove(e);
        applicationUser.Balance += e.Price;

        e.Attendees.Remove(applicationUser);

        e.UpdatedAt = DateTime.UtcNow;
        applicationUser.UpdatedAt = DateTime.UtcNow;


        _unitOfWork.EventRepository.Update(e);
        _unitOfWork.ApplicationUserRepository.Update(applicationUser);
        await _unitOfWork.CompleteAsync();
        return Messages.Success.CancelEvent;
    }
}