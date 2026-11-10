namespace KodaEWallet.DTOs;
public class TransactionHistoryResponse
{
    public string WalletNumber { get; set; } = "";
    public string Filter { get; set; } = "";
    public int TotalCount { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public List<TransactionResponse> Transactions { get; set; } = [];
}