using KodaEWallet.DTOs;
using KodaEWallet.Exceptions;
using KodaEWallet.Interfaces;
using KodaEWallet.Models;
using KodaEWallet.Repositories;

namespace KodaEWallet.Services;

public class WalletService(IWalletRepository walletRepo, ITransactionRepository trxRepo) : IWalletService
{
    public async Task<TransactionHistoryResponse> GetHistoryAsync(int userId, HistoryFilter filter, CancellationToken ct)
    {
        Wallet wallet = await FindWalletByUserAsync(userId, ct);
        List<Transaction> transactions = await trxRepo.GetByWalletAsync(wallet.Id, filter, ct);

        List<TransactionResponse> items = transactions.Select(ToResponse).ToList();

        return new TransactionHistoryResponse
        {
            WalletNumber = wallet.WalletNumber,
            Filter = filter.ToString(),
            TotalCount = items.Count,
            TotalIncome = items.Where(t => t.Direction == "Income").Sum(t => t.Amount),
            TotalExpense = items.Where(t => t.Direction == "Expense").Sum(t => t.Amount),
            Transactions = items
        };
    }

    public async Task<WalletResponse> GetWalletAsync(int userId, CancellationToken ct)
    {
        Wallet wallet = await FindWalletByUserAsync(userId, ct);
        var (income, expense) = await trxRepo.GetTotalsAsync(wallet.Id, ct);

        return new WalletResponse
        {
            UserId = wallet.UserId,
            OwnerName = wallet.OwnerName,
            WalletNumber = wallet.WalletNumber,
            Balance = wallet.Balance,
            TotalIncome = income,
            TotalExpense = expense
        };
    }

    public async Task<TopUpResponse> TopUpAsync(int userId, TopUpRequest request, CancellationToken ct)
    {
        ValidateAmount(request.Amount);
        Wallet wallet = await FindWalletByUserAsync(userId, ct);

        string description = DefaultIfEmpty(request.Description, "Top up saldo");
        decimal balanceAfter = await trxRepo.TopUpAsync(wallet.Id, request.Amount, description, ct);

        return new TopUpResponse
        {
            WalletNumber = wallet.WalletNumber,
            Amount = request.Amount,
            BalanceAfter = balanceAfter
        };
    }

    public async Task<TransferResponse> TransferAsync(TransferRequest request, CancellationToken ct)
    {
        var (sender, receiver) = await CheckTransferAsync(request, ct);

        string description = DefaultIfEmpty(request.Description, "Transfer saldo");
        decimal senderBalanceAfter = await trxRepo.TransferAsync(
            sender.Id, receiver.Id, request.Amount, description, ct);

        return new TransferResponse
        {
            SenderName = sender.OwnerName,
            SenderWalletNumber = sender.WalletNumber,
            ReceiverName = receiver.OwnerName,
            ReceiverWalletNumber = receiver.WalletNumber,
            Amount = request.Amount,
            SenderBalanceAfter = senderBalanceAfter
        };
    }

    public async Task<TransferPreviewResponse> ValidateTransferAsync(TransferRequest request, CancellationToken ct)
    {
        var (sender, receiver) = await CheckTransferAsync(request, ct);

        return new TransferPreviewResponse
        {
            SenderUserId = sender.UserId,
            SenderName = sender.OwnerName,
            SenderBalance = sender.Balance,
            ReceiverWalletNumber = receiver.WalletNumber,
            ReceiverName = receiver.OwnerName,
            Amount = request.Amount,
            Description = request.Description
        };
    }

    private async Task<(Wallet Sender, Wallet Receiver)> CheckTransferAsync(TransferRequest request, CancellationToken ct)
    {
        Wallet sender = await FindWalletByUserAsync(request.SenderUserId, ct);

        string receiverNumber = request.ReceiverWalletNumber.Trim().ToUpperInvariant();
        Wallet receiver = await walletRepo.GetByNumberAsync(receiverNumber, ct)
            ?? throw new NotFoundException($"Wallet dengan nomor {receiverNumber} tidak ditemukan");

        if (receiver.Id == sender.Id)
        {
            throw new BadRequestException("Tidak bisa transfer ke wallet sendiri");
        }

        ValidateAmount(request.Amount);

        if (request.Amount > sender.Balance)
        {
            throw new InsufficientBalanceException(sender.Balance, request.Amount);
        }

        return (sender, receiver);
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new BadRequestException("Nominal harus lebih dari 0");
        }
        if (decimal.Round(amount, 2) != amount)
        {
            throw new BadRequestException("Nominal maksimal 2 angka di belakang koma");
        }
    }


    private async Task<Wallet> FindWalletByUserAsync(int userId, CancellationToken ct)
    {
        return await walletRepo.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException($"Wallet untuk user dengan id {userId} tidak ditemukan");
    }

    private static TransactionResponse ToResponse(Transaction t) => new TransactionResponse
    {
        Id = t.Id,
        Type = t.Type,
        Label = GetLabel(t),
        Direction = t.Direction,
        SenderWalletNumber = t.SenderWalletNumber,
        SenderName = t.SenderName,
        ReceiverWalletNumber = t.ReceiverWalletNumber,
        ReceiverName = t.ReceiverName,
        Amount = t.Amount,
        Description = t.Description,
        CreatedAt = t.CreatedAt
    };

    private static string GetLabel(Transaction t)
    {
        if (t.Type == "TOPUP")
        {
            return "Top Up";
        }
        return t.Direction == "Income" ? "Transfer Masuk" : "Transfer Keluar";
    }
    private static string DefaultIfEmpty(string? text, string fallback) =>
    string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
}