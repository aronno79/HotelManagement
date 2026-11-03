using HotelApi.Data;
using HotelApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly RoomRepository _repo;
    public RoomsController(RoomRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var room = await _repo.GetByIdAsync(id);
        return room == null ? NotFound() : Ok(room);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Room room)
    {
        room.Id = await _repo.CreateAsync(room);
        return CreatedAtAction(nameof(Get), new { id = room.Id }, room);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Room room)
    {
        if (id != room.Id) return BadRequest();
        return await _repo.UpdateAsync(room) ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _repo.DeleteAsync(id) ? NoContent() : NotFound();
}