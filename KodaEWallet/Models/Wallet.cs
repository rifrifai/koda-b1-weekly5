using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KodaEWallet.Models;
public sealed class Wallet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string WalletNumber { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public decimal Balance { get; set; }
}