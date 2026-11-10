using KodaEWallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace KodaEWallet.Controllers.MVC;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(IUserService userService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await userService.GetAllAsync(ct);

        return View(users);
    }
}