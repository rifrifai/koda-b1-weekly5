using KodaEWallet.Models;

namespace KodaEWallet.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, int? exceptUserId, CancellationToken ct);
    Task<User> CreateWithWalletAsync(User user, CancellationToken ct);
    Task<bool> UpdateAsync(User user, CancellationToken ct);
}
