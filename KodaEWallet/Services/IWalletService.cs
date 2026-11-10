using KodaEWallet.DTOs;
using KodaEWallet.Models;

namespace KodaEWallet.Services;

public interface IWalletService
{
    Task<WalletResponse> GetWalletAsync(int userId, CancellationToken ct);
    Task<TopUpResponse> TopUpAsync(int userId, TopUpRequest request, CancellationToken ct);
    Task<TransferPreviewResponse> ValidateTransferAsync(TransferRequest request, CancellationToken ct);
    Task<TransferResponse> TransferAsync(TransferRequest request, CancellationToken ct);
    Task<TransactionHistoryResponse> GetHistoryAsync(int userId, HistoryFilter filter, CancellationToken ct);
}
