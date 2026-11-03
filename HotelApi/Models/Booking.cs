namespace HotelApi.Models;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Status { get; set; } = "Booked";   // ? deya optional
    public string? RoomNumber { get; set; }
    public string? GuestName { get; set; }
}