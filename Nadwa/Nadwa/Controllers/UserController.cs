using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nadwa.Models;
using Nadwa.Services.ApplicationUser;
using Nadwa.Services.Event;
using Nadwa.Utilites;
using NuGet.Protocol.Plugins;

namespace Nadwa.Controllers;
// [Authorize(Roles = "User")]

public class UserController : Controller{
    private IEventService _eventService;
    private IApplicationUserService _applicationUserService;
    private UserManager<ApplicationUser> _userManager;

    public UserController(IEventService eventService, IApplicationUserService applicationUserService, UserManager<ApplicationUser> userManager) {
        _eventService = eventService;
        _applicationUserService = applicationUserService;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> EventDetails(string id) {
        return PartialView(await _eventService.GetEventByIdAsync(id));
    }
    
    public async Task<IActionResult> CancelEvent(string id) {
        
        TempData["Message"] = await _applicationUserService.CancelEventAsync(await _userManager.GetUserAsync(User), await _eventService.GetEventByIdAsync(id));
        return RedirectToAction((string?)TempData["USER_CURR_PAGE"]);
        
    }
    
    public async Task<IActionResult> BookEvent(string id) {
        
        var msg = await _applicationUserService.BookEventAsync(await _userManager.GetUserAsync(User), await _eventService.GetEventByIdAsync(id));
        
        if (msg == Messages.Success.BookingEvent)
            return PartialView("BookingCompletion");

        TempData["Message"] = msg;
        return RedirectToAction((string?)TempData["USER_CURR_PAGE"]);

    }
    [HttpGet]
    public async Task<IActionResult> AllEvents([FromQuery] SearchQueryViewModel? query) {
        var q = HttpContext.Request.Query;
        string? searchQuery = q["Query"];
        Decimal.TryParse(q["MinPrice"], out var minPrice);
        Decimal.TryParse(q["MaxPrice"], out var maxPrice);
        DateTime.TryParse(q["FromDate"], out var fromDate);
        DateTime.TryParse(q["ToDate"], out var toDate);
        int.TryParse(q["Page"], out var page);


        var searchViewModel = new SearchQueryViewModel {
            Query = searchQuery,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page
        };
        if (q.Count == 0) searchViewModel = new SearchQueryViewModel();

        if (ModelState.IsValid) {
            ViewBag.lst = new List<Event>();
            ViewBag.lst = await _eventService.GetEventsUsingSearchViewModelAsync(searchViewModel);

        }


        return View(searchViewModel);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> UserEvents([FromQuery] SearchQueryViewModel? query) {
        var q = HttpContext.Request.Query;
        string? searchQuery = q["Query"];
        Decimal.TryParse(q["MinPrice"], out var minPrice);
        Decimal.TryParse(q["MaxPrice"], out var maxPrice);
        DateTime.TryParse(q["FromDate"], out var fromDate);
        DateTime.TryParse(q["ToDate"], out var toDate);
        int.TryParse(q["Page"], out var page);


        var searchViewModel = new SearchQueryViewModel {
            Query = searchQuery,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page
        };
        if (q.Count == 0) searchViewModel = new SearchQueryViewModel();
        var userEvents = (await _userManager.GetUserAsync(User))?.Events;
        
        
        if (ModelState.IsValid) {
            ViewBag.lst = new List<Event>();
            if (userEvents is null) userEvents = new List<Event>();

            ViewBag.lst = await _eventService.GetEventsUsingSearchViewModelAsync(searchViewModel, events:userEvents);
        }


        return View(searchViewModel);
    }
    
}