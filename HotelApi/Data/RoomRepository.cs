using HotelApi.Models;
using Microsoft.Data.SqlClient;

namespace HotelApi.Data;

public class RoomRepository
{
    private readonly string _cs;
    public RoomRepository(IConfiguration config) => _cs = config.GetConnectionString("DefaultConnection")!;

    private static Room Map(SqlDataReader r) => new Room
    {
        Id = (int)r["Id"],
        RoomNumber = (string)r["RoomNumber"],
        RoomType = (string)r["RoomType"],
        PricePerNight = (decimal)r["PricePerNight"],
        IsAvailable = (bool)r["IsAvailable"]
    };

    public async Task<List<Room>> GetAllAsync()
    {
        var list = new List<Room>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("SELECT * FROM Rooms ORDER BY RoomNumber", con);
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(Map(r));
        return list;
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("SELECT * FROM Rooms WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? Map(r) : null;
    }

    public async Task<int> CreateAsync(Room room)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
            INSERT INTO Rooms (RoomNumber, RoomType, PricePerNight, IsAvailable)
            OUTPUT INSERTED.Id
            VALUES (@RoomNumber, @RoomType, @Price, @IsAvailable)", con);
        cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
        cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
        cmd.Parameters.AddWithValue("@Price", room.PricePerNight);
        cmd.Parameters.AddWithValue("@IsAvailable", room.IsAvailable);
        await con.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task<bool> UpdateAsync(Room room)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
            UPDATE Rooms SET RoomNumber=@RoomNumber, RoomType=@RoomType,
                   PricePerNight=@Price, IsAvailable=@IsAvailable
            WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", room.Id);
        cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
        cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
        cmd.Parameters.AddWithValue("@Price", room.PricePerNight);
        cmd.Parameters.AddWithValue("@IsAvailable", room.IsAvailable);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("DELETE FROM Rooms WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}