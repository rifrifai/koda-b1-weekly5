using KodaEWallet.DTOs;
using KodaEWallet.Exceptions;
using KodaEWallet.Helpers;
using KodaEWallet.Models;
using KodaEWallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace KodaEWallet.Controllers.MVC;

[ApiExplorerSettings(IgnoreApi = true)]
public class WalletController(IWalletService walletService) : Controller
{
    public async Task<IActionResult> Dashboard(int userId, CancellationToken ct)
    {
        try
        {
            return View(await walletService.GetWalletAsync(userId, ct));
        }
        catch (AppException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> TopUp(int userId, CancellationToken ct)
    {
        await LoadWalletAsync(userId, ct);
        return View(new TopUpRequest());
    }
    [HttpPost]
    public async Task<IActionResult> TopUp(int userId, TopUpRequest request, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            try
            {
                TopUpResponse result = await walletService.TopUpAsync(userId, request, ct);
                TempData["Success"] = $"Top up {Format.Rupiah(result.Amount)} berhasil. Saldo sekarang {Format.Rupiah(result.BalanceAfter)}";
                return RedirectToAction(nameof(TopUp), new { userId });
            }
            catch (AppException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadWalletAsync(userId, ct);
        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> Transfer(int userId, CancellationToken ct)
    {
        await LoadWalletAsync(userId, ct);
        return View(new TransferRequest { SenderUserId = userId });
    }
    [HttpPost]
    public async Task<IActionResult> Transfer(int userId, TransferRequest request, CancellationToken ct)
    {
        request.SenderUserId = userId;

        if (ModelState.IsValid)
        {
            try
            {
                TransferPreviewResponse preview = await walletService.ValidateTransferAsync(request, ct);
                return View("TransferConfirm", preview);
            }
            catch (AppException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        await LoadWalletAsync(userId, ct);
        return View(request);
    }
    [HttpPost]
    public async Task<IActionResult> ConfirmTransfer(int userId, TransferRequest request, CancellationToken ct)
    {
        request.SenderUserId = userId;

        try
        {
            TransferResponse result = await walletService.TransferAsync(request, ct);
            TempData["Success"] = $"Transfer {Format.Rupiah(result.Amount)} ke {result.ReceiverName} ({result.ReceiverWalletNumber}) berhasil. Sisa saldo {Format.Rupiah(result.SenderBalanceAfter)}";
            return RedirectToAction(nameof(Dashboard), new { userId });
        }
        catch (AppException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Transfer), new { userId });
        }
    }

    public async Task<IActionResult> History(int userId, CancellationToken ct, HistoryFilter filter = HistoryFilter.All)
    {
        try
        {
            ViewBag.UserId = userId;
            return View(await walletService.GetHistoryAsync(userId, filter, ct));
        }
        catch (AppException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    private async Task LoadWalletAsync(int userId, CancellationToken ct)
    {
        ViewBag.Wallet = await walletService.GetWalletAsync(userId, ct);
    }
}