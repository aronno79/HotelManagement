using HotelApi.Data;
using HotelApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingRepository _repo;
    public BookingsController(BookingRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Booking booking)
    {
        var (ok, error, id) = await _repo.CreateAsync(booking);
        if (!ok) return BadRequest(error);
        booking.Id = id;
        return Ok(booking);
    }

    // PUT api/Bookings/5/status?status=CheckedIn
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        var allowed = new[] { "Booked", "CheckedIn", "CheckedOut", "Cancelled" };
        if (!allowed.Contains(status)) return BadRequest("Invalid status.");
        return await _repo.UpdateStatusAsync(id, status) ? NoContent() : NotFound();
    }
}