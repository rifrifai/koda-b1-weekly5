using System.ComponentModel.DataAnnotations;

namespace KodaEWallet.DTOs;
public class TransferRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "SenderUserId tidak valid")]
    public int SenderUserId { get; set; }

    [Required(ErrorMessage = "Nomor wallet tujuan wajib diisi")]
    [StringLength(20, ErrorMessage = "Nomor wallet maksimal 20 karakter")]
    public string ReceiverWalletNumber { get; set; } = "";

    [Range(0.01, 1_000_000_000, ErrorMessage = "Nominal harus lebih dari 0 dan maksimal Rp1.000.000.000")]
    public decimal Amount { get; set; }

    [StringLength(200, ErrorMessage = "Keterangan maksimal 200 karakter")]
    public string? Description { get; set; }
}