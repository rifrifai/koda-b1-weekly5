namespace KodaEWallet.DTOs;
public class TopUpResponse
{
    public string WalletNumber { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
}