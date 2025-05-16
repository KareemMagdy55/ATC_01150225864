using Nadwa.Models;

namespace Nadwa.Services.Event;

public interface IEventService {
    Task<IEnumerable<Models.Event>>? GetEventsPageAsync(int page = 1);
    Task<IEnumerable<Models.Event>>? GetAllEventsAsync();
    Task<Models.Event?> GetEventByIdAsync(string id);
    Task<string> UpdateEvent(Models.Event? updatedEvent, IFormFile? file);
    Task<string> AddEventAsync(Models.Event? e, IFormFile? file);
    Task<string> DeleteEvent(Models.Event? e);

    Task<IEnumerable<Models.Event>> GetEventsPagedUsingSearchQueryAsync(
        int page = 1,
        string? searchQuery = null,
        IEnumerable<Models.Event>? events = null);

    Task<IEnumerable<Models.Event>> GetEventsPagedUsingPriceFilter(
        decimal minPrice = decimal.Zero,
        decimal maxPrice = 10000000000,
        int page = 1,
        IEnumerable<Models.Event>? events = null);

    Task<IEnumerable<Models.Event>> GetEventsPagedUsingDateRangeAsync(
        int page = 1,
        DateTime? from = null,
        DateTime? to = null,
        IEnumerable<Models.Event>? events = null);

    Task<IEnumerable<Models.ApplicationUser>?> GetEventAttendeesPagedAsync(Models.Event e, int page = 1);

    public Task<IEnumerable<Models.Event>> GetEventsUsingSearchViewModelAsync(
        SearchQueryViewModel? searchQueryViewModel,
        IEnumerable<Models.Event>? events = null);
}