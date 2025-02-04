using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AppVecinos.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AuthService _authService;

    public AccountController(ILogger<HomeController> logger, AuthService authService)
    {
         _logger = logger;
        _authService = authService;
    }

    public IActionResult Login()
    {
        ViewData["HideMenu"] = true;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var response = await _authService.LoginAsync(username, password);
         
        if (response != null)
        {
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(response);
            if (authResponse != null)
            {
                var token = authResponse.token;
                Console.WriteLine($"Token generado: {token}");
                HttpContext.Session.SetString("AuthToken", token);
            }
            else
            {
                ViewBag.ErrorMessage = "Error al procesar la respuesta de autenticación.";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("AuthToken");
        return RedirectToAction("Login", "Account");
    }
}
