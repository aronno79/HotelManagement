using HotelApi.Data;
using HotelApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly GuestRepository _repo;
    public GuestsController(GuestRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Guest guest)
    {
        guest.Id = await _repo.CreateAsync(guest);
        return Ok(guest);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _repo.DeleteAsync(id) ? NoContent() : NotFound();
}