using System.ComponentModel.DataAnnotations;

namespace KodaEWallet.DTOs;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Nama wajib diisi")]
    [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    [StringLength(150, ErrorMessage = "Email maksimal 150 karakter")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Nomor telepon wajib diisi")]
    // [RegularExpression(@"^[0-9]{9,13}$", ErrorMessage = "Nomor telepon harus 9-13 digit angka")]
    [Phone]
    public string Phone { get; set; } = "";
}