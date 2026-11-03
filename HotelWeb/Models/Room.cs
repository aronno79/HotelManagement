namespace HotelWeb.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = "";
    public string RoomType { get; set; } = "Single";
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; } = true;
}