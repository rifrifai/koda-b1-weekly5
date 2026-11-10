namespace KodaEWallet.DTOs;

public class WalletResponse
{
    public int UserId { get; set; }
    public string OwnerName { get; set; } = "";
    public string WalletNumber { get; set; } = "";
    public decimal Balance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
}