using System.Data;
using KodaEWallet.Data;
using KodaEWallet.Repositories;
using KodaEWallet.Models;
using Microsoft.Data.SqlClient;

namespace KodaEWallet.Repositories;

public class UserRepository(DbConnection _db) : IUserRepository
{
    public async Task<User> CreateWithWalletAsync(User user, CancellationToken ct)
    {
        const string insertUserSql =
            "INSERT INTO Users (Name, Email, Phone) OUTPUT INSERTED.Id VALUES (@Name, @Email, @Phone)";
        const string insertWalletSql =
            "INSERT INTO Wallets (UserId, WalletNumber, Balance) VALUES (@UserId, @WalletNumber, 0)";

        await using var conn = await _db.OpenAsync(ct);
        await using var tx = (SqlTransaction)await conn.BeginTransactionAsync(ct);

        await using (var cmd = new SqlCommand(insertUserSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Phone", user.Phone);
            user.Id = (int)(await cmd.ExecuteScalarAsync(ct))!;
        }

        await using (var cmd = new SqlCommand(insertWalletSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@UserId", user.Id);
            cmd.Parameters.AddWithValue("@WalletNumber", $"WAL{user.Id:D3}");
            await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, int? exceptUserId, CancellationToken ct)
    {
        const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND (@ExceptId IS NULL OR Id <> @ExceptId)";

        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.Add(new SqlParameter("@ExceptId", SqlDbType.Int) { Value = (object?)exceptUserId ?? DBNull.Value });

        int count = (int)(await cmd.ExecuteScalarAsync(ct))!;
        return count > 0;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken ct)
    {
        const string sql = "SELECT Id, Name, Email, Phone FROM Users ORDER BY Id";

        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var users = new List<User>();
        while (await reader.ReadAsync(ct))
        {
            users.Add(ReadUser(reader));
        }
        return users;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        const string sql = "SELECT Id, Name, Email, Phone FROM Users WHERE Id = @Id";

        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            return ReadUser(reader);
        }
        return null;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken ct)
    {
        const string sql = "UPDATE Users SET Name = @Name, Email = @Email, Phone = @Phone WHERE Id = @Id";

        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", user.Id);
        cmd.Parameters.AddWithValue("@Name", user.Name);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@Phone", user.Phone);

        int rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    private static User ReadUser(SqlDataReader reader) => new()
    {
        Id = (int)reader["Id"],
        Name = (string)reader["Name"],
        Email = (string)reader["Email"],
        Phone = (string)reader["Phone"]
    };
}