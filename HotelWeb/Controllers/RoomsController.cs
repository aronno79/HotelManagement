using HotelWeb.Models;
using HotelWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers;

public class RoomsController : Controller
{
    private readonly ApiService _api;
    public RoomsController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
        => View(await _api.GetAsync<List<Room>>("api/Rooms") ?? new List<Room>());

    public IActionResult Create() => View(new Room());

    [HttpPost]
    public async Task<IActionResult> Create(Room room)
    {
        await _api.PostAsync("api/Rooms", room);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var room = await _api.GetAsync<Room>($"api/Rooms/{id}");
        return room == null ? NotFound() : View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Room room)
    {
        await _api.PutAsync($"api/Rooms/{room.Id}", room);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"api/Rooms/{id}");
        return RedirectToAction(nameof(Index));
    }
}