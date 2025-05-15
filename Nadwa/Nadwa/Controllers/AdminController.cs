using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nadwa.Models;
using Nadwa.Services.Event;
using Nadwa.Utilites;
using static System.DateTime;
using static System.Decimal;

namespace Nadwa.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
    private IEventService _eventService;

    public AdminController(IEventService eventService) {
        _eventService = eventService;
    }


    public async Task<IActionResult> CreateEvent(Event e, IFormFile? file ) {
        if (!ModelState.IsValid) return View(e);

        TempData["Message"] =  await _eventService.AddEventAsync(e, file);
        return RedirectToAction("Index");
    }
 
    
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery]SearchQueryViewModel? query) {
        
        var q = HttpContext.Request.Query;
        string? searchQuery = q["Query"];
        Decimal.TryParse(q["MinPrice"], out var minPrice);
        Decimal.TryParse(q["MaxPrice"], out var maxPrice);
        DateTime.TryParse(q["FromDate"], out var fromDate);
        DateTime.TryParse(q["ToDate"], out var toDate);
        int.TryParse(q["Page"], out var page);
        
        
        
        var searchViewModel = new SearchQueryViewModel
        {
            Query = searchQuery,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page
        };
        if (q.Count == 0) searchViewModel = new SearchQueryViewModel();
        
        if (ModelState.IsValid) {
            Console.WriteLine("Query : " + query?.Query);
            Console.WriteLine("Max price : " + query?.MaxPrice);
            ViewBag.lst = new List<Event>();
            ViewBag.lst = await _eventService.GetEventsUsingSearchViewModelAsync(searchViewModel);
        }


        return View(query);
    }
}