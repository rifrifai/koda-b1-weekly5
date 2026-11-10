namespace KodaEWallet.DTOs;
public class TransferPreviewResponse
{
    public int SenderUserId { get; set; }
    public string SenderName { get; set; } = "";
    public decimal SenderBalance { get; set; }

    public string ReceiverWalletNumber { get; set; } = "";
    public string ReceiverName { get; set; } = "";

    public decimal Amount { get; set; }
    public string? Description { get; set; }
}