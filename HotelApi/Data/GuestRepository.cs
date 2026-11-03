using HotelApi.Models;
using Microsoft.Data.SqlClient;

namespace HotelApi.Data;

public class GuestRepository
{
    private readonly string _cs;
    public GuestRepository(IConfiguration config) => _cs = config.GetConnectionString("DefaultConnection")!;

    public async Task<List<Guest>> GetAllAsync()
    {
        var list = new List<Guest>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("SELECT * FROM Guests ORDER BY FullName", con);
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new Guest
            {
                Id = (int)r["Id"],
                FullName = (string)r["FullName"],
                Phone = (string)r["Phone"],
                Email = r["Email"] as string,
                NidOrPassport = r["NidOrPassport"] as string
            });
        }
        return list;
    }

    public async Task<int> CreateAsync(Guest g)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
            INSERT INTO Guests (FullName, Phone, Email, NidOrPassport)
            OUTPUT INSERTED.Id
            VALUES (@FullName, @Phone, @Email, @Nid)", con);
        cmd.Parameters.AddWithValue("@FullName", g.FullName);
        cmd.Parameters.AddWithValue("@Phone", g.Phone);
        cmd.Parameters.AddWithValue("@Email", (object?)g.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Nid", (object?)g.NidOrPassport ?? DBNull.Value);
        await con.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("DELETE FROM Guests WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}