using HotelApi.Models;
using Microsoft.Data.SqlClient;

namespace HotelApi.Data;

public class BookingRepository
{
    private readonly string _cs;
    public BookingRepository(IConfiguration config) => _cs = config.GetConnectionString("DefaultConnection")!;

    public async Task<List<Booking>> GetAllAsync()
    {
        var list = new List<Booking>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
            SELECT b.*, r.RoomNumber, g.FullName AS GuestName
            FROM Bookings b
            JOIN Rooms r ON r.Id = b.RoomId
            JOIN Guests g ON g.Id = b.GuestId
            ORDER BY b.Id DESC", con);
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new Booking
            {
                Id = (int)r["Id"],
                RoomId = (int)r["RoomId"],
                GuestId = (int)r["GuestId"],
                CheckIn = (DateTime)r["CheckIn"],
                CheckOut = (DateTime)r["CheckOut"],
                TotalPrice = (decimal)r["TotalPrice"],
                Status = (string)r["Status"],
                RoomNumber = (string)r["RoomNumber"],
                GuestName = (string)r["GuestName"]
            });
        }
        return list;
    }

    public async Task<(bool Ok, string? Error, int Id)> CreateAsync(Booking b)
    {
        if (b.CheckOut <= b.CheckIn)
            return (false, "Check-out date must be after check-in date.", 0);

        using var con = new SqlConnection(_cs);
        await con.OpenAsync();

        // 1. Room er price ana
        decimal price;
        using (var c1 = new SqlCommand("SELECT PricePerNight FROM Rooms WHERE Id=@Id", con))
        {
            c1.Parameters.AddWithValue("@Id", b.RoomId);
            var o = await c1.ExecuteScalarAsync();
            if (o == null) return (false, "Room not found.", 0);
            price = (decimal)o;
        }

        // 2. Ei date e room age thekei booked kina check
        using (var c2 = new SqlCommand(@"
            SELECT COUNT(*) FROM Bookings
            WHERE RoomId=@RoomId AND Status IN ('Booked','CheckedIn')
              AND @CheckIn < CheckOut AND @CheckOut > CheckIn", con))
        {
            c2.Parameters.AddWithValue("@RoomId", b.RoomId);
            c2.Parameters.AddWithValue("@CheckIn", b.CheckIn);
            c2.Parameters.AddWithValue("@CheckOut", b.CheckOut);
            if ((int)(await c2.ExecuteScalarAsync())! > 0)
                return (false, "Room is already booked for these dates.", 0);
        }

        // 3. Total price = raat * price
        var nights = (b.CheckOut.Date - b.CheckIn.Date).Days;
        if (nights < 1) nights = 1;
        var total = nights * price;

        // 4. Insert
        using var c3 = new SqlCommand(@"
            INSERT INTO Bookings (RoomId, GuestId, CheckIn, CheckOut, TotalPrice, Status)
            OUTPUT INSERTED.Id
            VALUES (@RoomId, @GuestId, @CheckIn, @CheckOut, @Total, 'Booked')", con);
        c3.Parameters.AddWithValue("@RoomId", b.RoomId);
        c3.Parameters.AddWithValue("@GuestId", b.GuestId);
        c3.Parameters.AddWithValue("@CheckIn", b.CheckIn);
        c3.Parameters.AddWithValue("@CheckOut", b.CheckOut);
        c3.Parameters.AddWithValue("@Total", total);
        var id = (int)(await c3.ExecuteScalarAsync())!;
        return (true, null, id);
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
            UPDATE Bookings SET Status=@S WHERE Id=@Id;
            UPDATE Rooms SET IsAvailable = CASE WHEN @S='CheckedIn' THEN 0 ELSE 1 END
            WHERE Id = (SELECT RoomId FROM Bookings WHERE Id=@Id);", con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@S", status);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}