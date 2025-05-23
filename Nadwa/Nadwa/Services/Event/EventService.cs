﻿using System.Collections;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using Nadwa.Data.Repositories.Interface;
using Nadwa.Models;
using Nadwa.Utilites;

namespace Nadwa.Services.Event;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;

    public EventService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
    {
        _unitOfWork = unitOfWork;
        _env = env;
    }

    public async Task<IEnumerable<Models.Event>>? GetEventsPageAsync(int page = 1)
    {
        return await _unitOfWork
            .EventRepository
            .GetPagedAsync(page);
    }


    public async Task<IEnumerable<Models.Event>>? GetAllEventsAsync()
    {
        return await _unitOfWork
            .EventRepository
            .GetAllAsync();
    }

    public async Task<Models.Event?> GetEventByIdAsync(string id)
    {
        var res = await _unitOfWork
            .EventRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == id);

        return res;
    }


    public async Task<string> UpdateEvent(Models.Event? updatedEvent, IFormFile? file)
    {
        var e = await _unitOfWork
            .EventRepository
            .GetFirstOrDefaultAsync(predicate: u => u.Id == updatedEvent.Id);

        if (e is null) return Messages.Fail.EventUpdate;
        e.Price = updatedEvent.Price;
        e.Name = updatedEvent.Name;
        e.Category = updatedEvent.Category;
        e.Venue = updatedEvent.Venue;
        e.Description = updatedEvent.Description;
        e.Date = DateTime.SpecifyKind(updatedEvent.Date, DateTimeKind.Local).ToUniversalTime();
        e.UpdatedAt = DateTime.UtcNow;

        if (e.ImageUrl != null)
        {
            await DeleteImageAsync(e.ImageUrl);
            await SaveImageAsync(file, e);
        }

        _unitOfWork.EventRepository.Update(e);
        await _unitOfWork.CompleteAsync();
        return Messages.Success.EventUpdate;
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            var fileName = Path.GetFileName(imageUrl);

            if (!string.IsNullOrEmpty(fileName))
            {
                var imageDirectory = Path.Combine(_env.WebRootPath, "images");
                var imagePath = Path.Combine(imageDirectory, fileName);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
        }
    }


    private async Task SaveImageAsync(IFormFile? file, Models.Event e)
    {
        if (file != null)
        {
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName);

            var imageDirectory = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            var filePath = Path.Combine(imageDirectory, fileName + extension);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            e.ImageUrl = "/images/" + fileName + extension; 
        }
    }

    public async Task<string> AddEventAsync(Models.Event? e, IFormFile? file)
    {
        if (e is null)
            return Messages.Fail.AddEvent;

        e.Date = DateTime.SpecifyKind(e.Date, DateTimeKind.Local).ToUniversalTime();

        await SaveImageAsync(file, e);


        await _unitOfWork
            .EventRepository
            .AddAsync(e);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.AddEvent;
    }

    public async Task<string> DeleteEvent(Models.Event? e)
    {
        if (e is null)
            return Messages.Fail.EventDelete;

        if (e.Date <= DateTime.UtcNow && e.Attendees is not null)
        {
            foreach (var user in e.Attendees)
            {
                user.Balance += e.Price;
            }
        }

        if (!string.IsNullOrEmpty(e.ImageUrl))
        {
          await  DeleteImageAsync(e.ImageUrl);
        }

        _unitOfWork
            .EventRepository
            .Remove(e);

        await _unitOfWork.CompleteAsync();
        return Messages.Success.EventDelete;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsPagedUsingSearchQueryAsync(int page = 1,
        string? searchQuery = null, IEnumerable<Models.Event>? events = null)
    {
        if (string.IsNullOrWhiteSpace(searchQuery)) return [];
        searchQuery = searchQuery.ToLower();

        var results = await _unitOfWork.EventRepository.GetPagedAsync(page,
            predicate: e =>
                e.Name.ToLower().Contains(searchQuery) ||
                e.Category.ToLower().Contains(searchQuery) ||
                e.Description.ToLower().Contains(searchQuery) ||
                e.Venue.ToLower().Contains(searchQuery) ||
                e.Tags.ToLower().Contains(searchQuery)
            , enumerable: events
        );

        return results;
    }


    public async Task<IEnumerable<Models.Event>> GetEventsPagedUsingPriceFilter(decimal minPrice = decimal.Zero,
        decimal maxPrice = 10000000000, int page = 1, IEnumerable<Models.Event>? events = null)
    {
        var results = await _unitOfWork.EventRepository.GetPagedAsync(page,
            predicate: e =>
                e.Price <= maxPrice && e.Price >= minPrice,
            enumerable: events
        );

        return results;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsPagedUsingDateRangeAsync(int page = 1, DateTime? from = null,
        DateTime? to = null, IEnumerable<Models.Event>? events = null)
    {
        var results = await _unitOfWork.EventRepository.GetPagedAsync(page,
            predicate: e =>
                e.Date <= to && e.Date >= from,
            enumerable: events
        );

        return results;
    }

    public async Task<IEnumerable<Models.ApplicationUser>?> GetEventAttendeesPagedAsync(Models.Event e, int page = 1)
    {
        var results = await _unitOfWork.ApplicationUserRepository.GetPagedAsync(page,
            enumerable: e.Attendees
        );

        return results;
    }

    public async Task<IEnumerable<Models.Event>> GetEventsUsingSearchViewModelAsync(
        SearchQueryViewModel? searchQueryViewModel, IEnumerable<Models.Event>? events = null)
    {
        searchQueryViewModel ??= new SearchQueryViewModel();

        if (searchQueryViewModel.FromDate.HasValue)
            searchQueryViewModel.FromDate = DateTime
                .SpecifyKind(searchQueryViewModel.FromDate.Value, DateTimeKind.Local).ToUniversalTime();

        if (searchQueryViewModel.ToDate.HasValue)
            searchQueryViewModel.ToDate = DateTime.SpecifyKind(searchQueryViewModel.ToDate.Value, DateTimeKind.Local)
                .ToUniversalTime();

        var byQuery = await GetEventsPagedUsingSearchQueryAsync(page: searchQueryViewModel.Page,
            searchQuery: searchQueryViewModel.Query);
        var byPrice = await GetEventsPagedUsingPriceFilter(minPrice: searchQueryViewModel.MinPrice,
            maxPrice: searchQueryViewModel.MaxPrice, page: searchQueryViewModel.Page);
        var byDate = await GetEventsPagedUsingDateRangeAsync(page: searchQueryViewModel.Page,
            from: searchQueryViewModel.FromDate, to: searchQueryViewModel.ToDate);


        IEnumerable<Models.Event> lst = new List<Models.Event>();
        lst = events != null
            ? events.Intersect(byPrice).Intersect(byDate).ToList()
            : byPrice.Intersect(byDate).ToList();

        if (!searchQueryViewModel.Query.IsNullOrEmpty())
            lst = lst.Intersect(byQuery);


        var filtered = await _unitOfWork.EventRepository.GetPagedAsync(
            page: 1,
            enumerable:
            lst
        );

        return filtered;
    }
}