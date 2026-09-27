using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Data;

namespace MVCPractice.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly BibliotecaContext _context;

    public AccountController(BibliotecaContext context)
    {
        _context = context;
    }

    [Route("")]
    [Route("Login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(string email)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
        {
            ViewBag.Error = "No existe un usuario con ese correo.";
            return View();
        }

        var rol = usuario.Role.Trim();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.Role, rol),
            new Claim("UsuarioId", usuario.UsuarioId.ToString())
        };

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [Route("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
