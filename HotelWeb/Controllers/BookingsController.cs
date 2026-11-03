using HotelWeb.Models;
using HotelWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.Controllers;

public class BookingsController : Controller
{
    private readonly ApiService _api;
    public BookingsController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
        => View(await _api.GetAsync<List<Booking>>("api/Bookings") ?? new List<Booking>());

    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new Booking { CheckIn = DateTime.Today, CheckOut = DateTime.Today.AddDays(1) });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Booking booking)
    {
        var res = await _api.PostAsync("api/Bookings", booking);
        if (!res.IsSuccessStatusCode)
        {
            ViewBag.Error = await res.Content.ReadAsStringAsync();
            await LoadLists();
            return View(booking);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Status(int id, string status)
    {
        await _api.PutNoBodyAsync($"api/Bookings/{id}/status?status={status}");
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLists()
    {
        var rooms = await _api.GetAsync<List<Room>>("api/Rooms") ?? new List<Room>();
        var guests = await _api.GetAsync<List<Guest>>("api/Guests") ?? new List<Guest>();
        ViewBag.Rooms = new SelectList(rooms, "Id", "RoomNumber");
        ViewBag.Guests = new SelectList(guests, "Id", "FullName");
    }
}