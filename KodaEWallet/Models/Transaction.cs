using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KodaEWallet.Models;
public sealed class Transaction
{
    public int Id { get; set; }
    public string Type { get; set; } = "";
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? SenderWalletNumber { get; set; }
    public string? SenderName { get; set; }
    public string ReceiverWalletNumber { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public string Direction { get; set; } = "";
}