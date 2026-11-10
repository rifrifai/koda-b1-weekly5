namespace KodaEWallet.Exceptions;
public class BadRequestException(string message) : AppException(message, StatusCodes.Status400BadRequest)
{
}
