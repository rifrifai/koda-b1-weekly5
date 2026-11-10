using KodaEWallet.Helpers;

namespace KodaEWallet.Exceptions;
public class InsufficientBalanceException(string message) : AppException(message, StatusCodes.Status422UnprocessableEntity)
{
    public InsufficientBalanceException(decimal balance, decimal amount)
        : this($"Saldo tidak cukup. Saldo Anda {Format.Rupiah(balance)}, nominal transfer {Format.Rupiah(amount)}") { }
}
