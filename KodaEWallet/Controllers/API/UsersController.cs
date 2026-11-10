using KodaEWallet.DTOs;
using KodaEWallet.Models;
using KodaEWallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace KodaEWallet.Controllers.API;
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(IUserService userService, IWalletService walletService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll(CancellationToken ct)
    {
        return Ok(await userService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken ct)
    {
        return Ok(await userService.GetByIdAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request, CancellationToken ct)
    {
        UserResponse user = await userService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Update(int id, UpdateUserRequest request, CancellationToken ct)
    {
        return Ok(await userService.UpdateAsync(id, request, ct));
    }

    [HttpGet("{id:int}/transactions")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionHistoryResponse>> GetTransactions(int id, CancellationToken ct, [FromQuery] HistoryFilter filter = HistoryFilter.All)
    {
        return Ok(await walletService.GetHistoryAsync(id, filter, ct));
    }
}