using KodaEWallet.Models;

namespace KodaEWallet.Repositories;
public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<Wallet?> GetByNumberAsync(string walletNumber, CancellationToken ct);
}