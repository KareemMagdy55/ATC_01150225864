using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nadwa.Models;
using Nadwa.Services.ApplicationUser;
using Nadwa.Services.Event;
using Nadwa.Utilites;
using static System.DateTime;
using static System.Decimal;

namespace Nadwa.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
    private IEventService _eventService;
    private IApplicationUserService _applicationUserService;

    public AdminController(IEventService eventService, IApplicationUserService applicationUserService) {
        _eventService = eventService;
        _applicationUserService = applicationUserService;
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(string id) {
        return View(await _eventService.GetEventByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> EditEvent(Event e, IFormFile? file) {
        if (ModelState.IsValid) {
            TempData["Message"] = await _eventService.UpdateEvent(e, Request.Form.Files["ImageUrl"]);
            return RedirectToAction("Index");
        }

        return View(e);
    }

    public async Task<IActionResult> DeleteEvent(string id) {
        TempData["Message"] = await _eventService.DeleteEvent(await _eventService.GetEventByIdAsync(id));
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> RemoveAttendee(string attendeeId, string eventId) {

        var ev = await _eventService.GetEventByIdAsync(eventId);
        var user = await _applicationUserService.GetUserByIdAsync(attendeeId);

        TempData["Message"] =  await _applicationUserService.CancelEventAsync(user, ev);

        return RedirectToAction("EditEvent", new { id = eventId });
    }


    [HttpGet]
    public async Task<IActionResult> EventDetails(string id) {
        return View(await _eventService.GetEventByIdAsync(id));
    }

    public async Task<IActionResult> CreateEvent(Event e, IFormFile? file) {
        if (!ModelState.IsValid) return View(e);
        TempData["Message"] = await _eventService.AddEventAsync(e, Request.Form.Files["ImageUrl"]);
        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SearchQueryViewModel? query) {
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
}