using KodaEWallet.DTOs;
using KodaEWallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace KodaEWallet.Controllers.API;

[ApiController]
[Route("api/v1/[controller]")]
public class TransfersController(IWalletService walletService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TransferResponse>> Transfer(TransferRequest request, CancellationToken ct)
    {
        return Ok(await walletService.TransferAsync(request, ct));
    }
}