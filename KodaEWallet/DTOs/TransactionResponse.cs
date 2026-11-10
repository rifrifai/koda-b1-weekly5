using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KodaEWallet.DTOs;
public class TransactionResponse
{
    public int Id { get; set; }
    public string Type { get; set; } = "";        // TOPUP / TRANSFER
    public string Label { get; set; } = "";       // Top Up / Transfer Masuk / Transfer Keluar
    public string Direction { get; set; } = "";   // Income / Expense
    public string? SenderWalletNumber { get; set; }
    public string? SenderName { get; set; }
    public string ReceiverWalletNumber { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}