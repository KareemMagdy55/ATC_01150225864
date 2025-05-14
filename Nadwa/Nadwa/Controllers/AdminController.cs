using Microsoft.AspNetCore.Mvc;
using Nadwa.Models;
using Nadwa.Services.Event;

namespace Nadwa.Controllers;

public class AdminController : Controller {
    private IEventService _eventService;

    public AdminController(IEventService eventService) {
        _eventService = eventService;
    }


    public IActionResult Index() {
        return View();
    }

    public async Task<IActionResult> ViewEventsPanel(SearchQueryViewModel? query = null) {
        query ??= new SearchQueryViewModel();
        var filteredEvents = await _eventService.GetEventsUsingSearchViewModelAsync(query);
        var lst = filteredEvents;
        ViewBag.isCountEqualZero = !filteredEvents.Any();
        ViewBag.lst = filteredEvents;
        
        return View(query);
    }
}