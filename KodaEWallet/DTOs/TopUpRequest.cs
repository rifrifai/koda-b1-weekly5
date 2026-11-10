using System.ComponentModel.DataAnnotations;

namespace KodaEWallet.DTOs;
public class TopUpRequest
{
    [Range(0.01, 1_000_000_000, ErrorMessage = "Nominal harus lebih dari 0 dan maksimal Rp1.000.000.000")]
    public decimal Amount { get; set; }

    [StringLength(200, ErrorMessage = "Keterangan maksimal 200 karakter")]
    public string? Description { get; set; }
}