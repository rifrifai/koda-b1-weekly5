using KodaEWallet.Data;
using KodaEWallet.Exceptions;
using KodaEWallet.Repositories;
using KodaEWallet.Models;
using Microsoft.Data.SqlClient;

namespace KodaEWallet.Repositories;

public class TransactionRepository(DbConnection db) : ITransactionRepository
{
    private readonly DbConnection _db = db;
    public async Task<List<Transaction>> GetByWalletAsync(int walletId, HistoryFilter filter, CancellationToken ct)
    {
        const string sql = @"
            SELECT t.Id, t.Type, t.Amount, t.Description, t.CreatedAt,
                sw.WalletNumber AS SenderNumber,   su.Name AS SenderName,
                rw.WalletNumber AS ReceiverNumber, ru.Name AS ReceiverName,
                CASE WHEN t.ReceiverWalletId = @WalletId THEN 'Income' ELSE 'Expense' END AS Direction
            FROM Transactions t
            LEFT JOIN Wallets sw ON sw.Id = t.SenderWalletId
            LEFT JOIN Users   su ON su.Id = sw.UserId
            JOIN Wallets rw ON rw.Id = t.ReceiverWalletId
            JOIN Users   ru ON ru.Id = rw.UserId
            WHERE (t.ReceiverWalletId = @WalletId AND @Filter <> 'Expense')
                OR (t.SenderWalletId   = @WalletId AND @Filter <> 'Income')
            ORDER BY t.CreatedAt DESC, t.Id DESC";

        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@WalletId", walletId);
        cmd.Parameters.AddWithValue("@Filter", filter.ToString());
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var list = new List<Transaction>();
        while (await reader.ReadAsync(ct))
        {
            list.Add(new Transaction
            {
                Id = (int)reader["Id"],
                Type = (string)reader["Type"],
                Amount = (decimal)reader["Amount"],
                Description = reader["Description"] as string,
                CreatedAt = (DateTime)reader["CreatedAt"],
                SenderWalletNumber = reader["SenderNumber"] as string,
                SenderName = reader["SenderName"] as string,
                ReceiverWalletNumber = (string)reader["ReceiverNumber"],
                ReceiverName = (string)reader["ReceiverName"],
                Direction = (string)reader["Direction"]
            });
        }
        return list;
    }

    public async  Task<(decimal Income, decimal Expense)> GetTotalsAsync(int walletId, CancellationToken ct)
    {
        const string sql = @"
            SELECT ISNULL(SUM(CASE WHEN ReceiverWalletId = @WalletId THEN Amount END), 0) AS TotalIncome,
                ISNULL(SUM(CASE WHEN SenderWalletId   = @WalletId THEN Amount END), 0) AS TotalExpense
            FROM Transactions
            WHERE ReceiverWalletId = @WalletId OR SenderWalletId = @WalletId";

        await using var conn = await _db.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@WalletId", walletId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        await reader.ReadAsync(ct);
        return ((decimal)reader["TotalIncome"], (decimal)reader["TotalExpense"]);
    }

    public async Task<decimal> TopUpAsync(int walletId, decimal amount, string description, CancellationToken ct)
    {
        const string updateSql =
            "UPDATE Wallets SET Balance = Balance + @Amount OUTPUT INSERTED.Balance WHERE Id = @WalletId";
        const string insertSql =
            @"INSERT INTO Transactions (SenderWalletId, ReceiverWalletId, Type, Amount, Description)
                VALUES (NULL, @WalletId, 'TOPUP', @Amount, @Description)";

        await using var conn = await _db.OpenAsync(ct);
        await using var tx = (SqlTransaction)await conn.BeginTransactionAsync(ct);

        decimal newBalance;
        await using (var cmd = new SqlCommand(updateSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@WalletId", walletId);
            cmd.Parameters.Add(SqlParams.Money("@Amount", amount));
            object? result = await cmd.ExecuteScalarAsync(ct);
            if (result is null)
            {
                throw new NotFoundException("Wallet tidak ditemukan");
            }
            newBalance = (decimal)result;
        }

        await using (var cmd = new SqlCommand(insertSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@WalletId", walletId);
            cmd.Parameters.Add(SqlParams.Money("@Amount", amount));
            cmd.Parameters.Add(SqlParams.Text("@Description", description));
            await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return newBalance;
    }

    public async Task<decimal> TransferAsync(int senderWalletId, int receiverWalletId, decimal amount, string description, CancellationToken ct)
    {
        const string withdrawSql =
            "UPDATE Wallets SET Balance = Balance - @Amount OUTPUT INSERTED.Balance WHERE Id = @SenderId AND Balance >= @Amount";
        const string depositSql =
            "UPDATE Wallets SET Balance = Balance + @Amount WHERE Id = @ReceiverId";
        const string insertSql =
            @"INSERT INTO Transactions (SenderWalletId, ReceiverWalletId, Type, Amount, Description)
                VALUES (@SenderId, @ReceiverId, 'TRANSFER', @Amount, @Description)";

        await using var conn = await _db.OpenAsync(ct);
        await using var tx = (SqlTransaction)await conn.BeginTransactionAsync(ct);

        decimal senderBalanceAfter;
        await using (var cmd = new SqlCommand(withdrawSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@SenderId", senderWalletId);
            cmd.Parameters.Add(SqlParams.Money("@Amount", amount));
            object? result = await cmd.ExecuteScalarAsync(ct);
            if (result is null)
            {
                throw new InsufficientBalanceException("Saldo tidak mencukupi");
            }
            senderBalanceAfter = (decimal)result;
        }

        await using (var cmd = new SqlCommand(depositSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@ReceiverId", receiverWalletId);
            cmd.Parameters.Add(SqlParams.Money("@Amount", amount));
            int rows = await cmd.ExecuteNonQueryAsync(ct);
            if (rows == 0)
            {
                throw new NotFoundException("Wallet penerima tidak ditemukan");
            }
        }

        await using (var cmd = new SqlCommand(insertSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("@SenderId", senderWalletId);
            cmd.Parameters.AddWithValue("@ReceiverId", receiverWalletId);
            cmd.Parameters.Add(SqlParams.Money("@Amount", amount));
            cmd.Parameters.Add(SqlParams.Text("@Description", description));
            await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return senderBalanceAfter;
    }
}