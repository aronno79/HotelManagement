using HotelWeb.Models;
using HotelWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers;

public class GuestsController : Controller
{
    private readonly ApiService _api;
    public GuestsController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
        => View(await _api.GetAsync<List<Guest>>("api/Guests") ?? new List<Guest>());

    public IActionResult Create() => View(new Guest());

    [HttpPost]
    public async Task<IActionResult> Create(Guest guest)
    {
        await _api.PostAsync("api/Guests", guest);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync($"api/Guests/{id}");
        return RedirectToAction(nameof(Index));
    }
}