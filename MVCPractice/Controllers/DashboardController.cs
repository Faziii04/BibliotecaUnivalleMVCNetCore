using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Data;
using MVCPractice.Models;
using MVCPractice.ViewModels;

namespace MVCPractice.Controllers;

[Authorize]
[Route("[controller]")]
public class DashboardController : Controller
{
    private readonly BibliotecaContext _context;

    public DashboardController(BibliotecaContext context)
    {
        _context = context;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
        var usuario = await _context.Users.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

        if (usuario == null)
        {
            return RedirectToAction("Logout", "Account");
        }

        var esBibliotecario = User.IsInRole("Administrador") || User.IsInRole("Bibliotecario");

        var modelo = new DashboardViewModel
        {
            Usuario = usuario,
            Rol = usuario.Role.Trim(),
            CantidadLibros = await _context.Books.CountAsync(),
            EjemplaresDisponibles = await _context.Books.SumAsync(b => (int?)b.Stock) ?? 0,
            PrestamosActivos = await _context.Loans.CountAsync(l => !l.IsReturned),
            PrestamosDevueltos = await _context.Loans.CountAsync(l => l.IsReturned)
        };

        if (User.IsInRole("Administrador"))
        {
            modelo.CantidadUsuarios = await _context.Users.CountAsync();
        }

        if (esBibliotecario)
        {
            modelo.Prestamos = await _context.Loans
                .Include(l => l.Libro)
                .Include(l => l.Usuario)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }
        else
        {
            modelo.Prestamos = await _context.Loans
                .Include(l => l.Libro)
                .Where(l => l.UsuarioId == usuarioId)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();

            modelo.Catalogo = await _context.Books
                .Where(b => b.Stock > 0)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        return View(modelo);
    }
}
