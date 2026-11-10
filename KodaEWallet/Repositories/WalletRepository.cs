using KodaEWallet.Data;
using KodaEWallet.Models;
using KodaEWallet.Repositories;
using Microsoft.Data.SqlClient;

namespace KodaEWallet.Repositories;

public class WalletRepository(DbConnection _db) : IWalletRepository
{
    private const string SelectWallet =
        @"SELECT w.Id, w.UserId, w.WalletNumber, w.Balance, u.Name AS OwnerName
            FROM Wallets w
            JOIN Users u ON u.Id = w.UserId ";

    public async Task<Wallet?> GetByNumberAsync(string walletNumber, CancellationToken ct)
    {
        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(SelectWallet + "WHERE w.WalletNumber = @WalletNumber", conn);
        cmd.Parameters.AddWithValue("@WalletNumber", walletNumber);
        return await ReadOneAsync(cmd, ct);
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(SelectWallet + "WHERE w.UserId = @UserId", conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        return await ReadOneAsync(cmd, ct);
    }

    private static async Task<Wallet?> ReadOneAsync(SqlCommand cmd, CancellationToken ct)
    {
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new Wallet
        {
            Id = (int)reader["Id"],
            UserId = (int)reader["UserId"],
            WalletNumber = (string)reader["WalletNumber"],
            OwnerName = (string)reader["OwnerName"],
            Balance = (decimal)reader["Balance"]
        };
    }
}