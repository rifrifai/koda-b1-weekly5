namespace KodaEWallet.DTOs;

public class TransferResponse
{
    public string SenderName { get; set; } = "";
    public string SenderWalletNumber { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public string ReceiverWalletNumber { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal SenderBalanceAfter { get; set; }
}
