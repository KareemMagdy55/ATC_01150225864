using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Nadwa.Data.Repositories.Interface;
using Nadwa.Models;
using Nadwa.Utilites;

namespace Nadwa.Services.Event;

public class EventService : IEventService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly Cloudinary _cloudinary;

    public EventService(IUnitOfWork unitOfWork, Cloudinary cloudinary) {
        _unitOfWork = unitOfWork;
        _cloudinary = cloudinary;
    }

    public async Task<IEnumerable<Models.Event>>? GetEventsPageAsync(int page = 1) {
        return await _unitOfWork
            .EventRepository
            .GetPagedAsync(page);
    }


    public async Task<IEnumerable<Models.Event>>? GetAllEventsAsync() {
        return await _unitOfWork
            .EventRepository
            .GetAllAsync();
    }

    public async Task<Models.Event?> GetEventByIdAsync(string id) {
        var res = await _unitOfWork
            .EventRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == id);

        return res;
    }


    public async Task<string> UpdateEvent(Models.Event? updatedEvent, string? imagePath = null) {
        var e = await _unitOfWork
            .EventRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == updatedEvent.Id);

        if (e is null) return Messages.Fail.EventUpdate;
        e.Price = updatedEvent.Price;
        e.Name = updatedEvent.Name;
        e.Category = updatedEvent.Category;
        e.Venue = updatedEvent.Venue;
        e.Description = updatedEvent.Description;
        e.Date = updatedEvent.Date;
        e.UpdatedAt = DateTime.UtcNow;
        if (imagePath != null) {
            var uploadParams = new ImageUploadParams() {
                File = new FileDescription(imagePath)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            e.ImageUrl = uploadResult.SecureUrl.ToString();
        }

        _unitOfWork.EventRepository.Update(e);
        await _unitOfWork.CompleteAsync();
        return Messages.Success.EventUpdate;
    }


    public async Task<string> AddEventAsync(Models.Event? e, string? imagePath = null) {
        if (e is null)
            return Messages.Fail.AddEvent;

        if (imagePath is not null) {
            var uploadParams = new ImageUploadParams() {
                File = new FileDescription(imagePath)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            e.ImageUrl = uploadResult.SecureUrl.ToString();
        }

        await _unitOfWork
            .EventRepository
            .AddAsync(e);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.AddEvent;
    }

    public async Task<string> DeleteEvent(Models.Event? e) {
        if (e is null)
            return Messages.Fail.EventDelete;


        _unitOfWork
            .EventRepository
            .Remove(e);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.EventDelete;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsPagedUsingSearchQueryAsync(int page = 1,
        string? searchQuery = null, IEnumerable<Models.Event>? events = null) {
        if (string.IsNullOrWhiteSpace(searchQuery)) return [];
        searchQuery = searchQuery.ToLower();

        var results = await _unitOfWork.EventRepository.GetPagedAsync(page,
            predicate: e =>
                e.Name.ToLower().Contains(searchQuery) ||
                e.Category.ToLower().Contains(searchQuery) ||
                e.Description.ToLower().Contains(searchQuery) ||
                e.Venue.ToLower().Contains(searchQuery)
            , enumerable: events
        );

        return results;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsPagedUsingPriceFilter(decimal minPrice = decimal.Zero,
        decimal maxPrice = 10000000000, int page = 1, IEnumerable<Models.Event>? events = null) {
        var results = await _unitOfWork.EventRepository.GetPagedAsync(page,
            predicate: e =>
                e.Price <= maxPrice && e.Price >= minPrice,
            enumerable: events
        );

        return results;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsPagedUsingDateRangeAsync(int page = 1, DateTime? from = null,
        DateTime? to = null, IEnumerable<Models.Event>? events = null) {
        var results = await _unitOfWork.EventRepository.GetPagedAsync(page,
            predicate: e =>
                e.Date <= to && e.Date >= from,
            enumerable: events
        );

        return results;
    }

    public async Task<IEnumerable<Models.ApplicationUser>?> GetEventAttendeesPagedAsync(Models.Event e, int page = 1) {
        var results = await _unitOfWork.ApplicationUserRepository.GetPagedAsync(page,
            enumerable: e.Attendees
        );

        return results;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsUsingSearchViewModelAsync(
        SearchQueryViewModel searchQueryViewModel) {
        var byQuery = await GetEventsPagedUsingSearchQueryAsync(page: searchQueryViewModel.Page,
            searchQuery: searchQueryViewModel.Query);
        var byPrice = await GetEventsPagedUsingPriceFilter(minPrice: searchQueryViewModel.MinPrice,
            maxPrice: searchQueryViewModel.MaxPrice, page: searchQueryViewModel.Page);
        var byDate = await GetEventsPagedUsingDateRangeAsync(page: searchQueryViewModel.Page,
            from: searchQueryViewModel.FromDate, to: searchQueryViewModel.ToDate);

        var filtered = await _unitOfWork.EventRepository.GetPagedAsync(
            page: 1,
            enumerable:
            byQuery.Intersect(byPrice)
                .Intersect(byDate)
        );

        return filtered;
    }
}