using KodaEWallet.DTOs;
using KodaEWallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace KodaEWallet.Controllers.API;

[ApiController]
[Route("api/v1/[controller]")]
public class WalletsController(IWalletService walletService) : ControllerBase
{
    [HttpGet("{userId:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponse>> GetWallet(int userId, CancellationToken ct)
    {
        return Ok(await walletService.GetWalletAsync(userId, ct));
    }

    [HttpPost("{userId:int}/topups")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TopUpResponse>> TopUp(int userId, TopUpRequest request, CancellationToken ct)
    {
        return Ok(await walletService.TopUpAsync(userId, request, ct));
    }
}