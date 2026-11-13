using KodaEWallet.Models;

namespace KodaEWallet.Repositories;
public interface ITransactionRepository
{
    Task<List<Transaction>> GetByWalletAsync(int walletId, HistoryFilter filter, CancellationToken ct);
    Task<(decimal Income, decimal Expense)> GetTotalsAsync(int walletId, CancellationToken ct);
    Task<decimal> TopUpAsync(int walletId, decimal amount, string description, CancellationToken ct);
    Task<decimal> TransferAsync(int senderWalletId, int receiverWalletId, decimal amount, string description, CancellationToken ct);
}