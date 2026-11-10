using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace KodaEWallet.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException)
        {
            httpContext.Response.StatusCode = 499;
            return true;
        }

        int status;
        string title;

        switch (exception)
        {
            case AppException app:
                status = app.StatusCode;
                title = app.Message;
                break;
            case SqlException sql when sql.Number is 2627 or 2601:
                status = StatusCodes.Status409Conflict;
                title = "Data sudah ada (duplikat)";
                break;
            case SqlException:
                status = StatusCodes.Status500InternalServerError;
                title = "Terjadi kesalahan pada database";
                break;
            default:
                status = StatusCodes.Status500InternalServerError;
                title = "Terjadi kesalahan pada server";
                break;
        }

        if (status >= 500)
        {
            _logger.LogError(exception, "Unhandled exception pada {Path}", httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning("{Message} ({Path})", exception.Message, httpContext.Request.Path);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
