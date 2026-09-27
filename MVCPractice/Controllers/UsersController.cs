using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Data;
using MVCPractice.Models;
using MVCPractice.ViewModels;

namespace MVCPractice.Controllers;

[Authorize(Roles = "Administrador")]
[Route("[controller]")]
public class UsersController : Controller
{
    private readonly BibliotecaContext _context;

    public UsersController(BibliotecaContext context)
    {
        _context = context;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index(string? buscar)
    {
        var consulta = _context.Users.Include(u => u.Loans).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(u => u.FirstName.Contains(buscar) || u.LastName.Contains(buscar) || u.Email.Contains(buscar));
        }

        ViewBag.Buscar = buscar;

        return View(await consulta.OrderBy(u => u.LastName).ToListAsync());
    }

    [Route("Details")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Users
            .Include(u => u.Loans)
            .ThenInclude(l => l.Libro)
            .FirstOrDefaultAsync(u => u.UsuarioId == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [Route("Create")]
    public IActionResult Create()
    {
        CargarRoles();
        return View(new UserFormModel { Role = "Usuario" });
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(UserFormModel formulario)
    {
        if (await _context.Users.AnyAsync(u => u.Email == formulario.Email))
        {
            ModelState.AddModelError("Email", "Ya existe un usuario con ese correo.");
        }

        if (!ModelState.IsValid)
        {
            CargarRoles();
            return View(formulario);
        }

        _context.Users.Add(new User
        {
            FirstName = formulario.FirstName,
            LastName = formulario.LastName,
            Email = formulario.Email,
            PhoneNumber = formulario.PhoneNumber,
            Address = formulario.Address,
            Birthday = formulario.Birthday,
            Role = formulario.Role
        });

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Usuario registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Route("Edit")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Users.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        CargarRoles();

        return View(new UserFormModel
        {
            UsuarioId = usuario.UsuarioId,
            FirstName = usuario.FirstName,
            LastName = usuario.LastName,
            Email = usuario.Email,
            PhoneNumber = usuario.PhoneNumber,
            Address = usuario.Address,
            Birthday = usuario.Birthday,
            Role = usuario.Role.Trim()
        });
    }

    [HttpPost]
    [Route("Edit")]
    public async Task<IActionResult> Edit(int id, UserFormModel formulario)
    {
        if (id != formulario.UsuarioId)
        {
            return NotFound();
        }

        var usuario = await _context.Users.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        if (await _context.Users.AnyAsync(u => u.Email == formulario.Email && u.UsuarioId != id))
        {
            ModelState.AddModelError("Email", "Ya existe otro usuario con ese correo.");
        }

        if (!ModelState.IsValid)
        {
            CargarRoles();
            return View(formulario);
        }

        usuario.FirstName = formulario.FirstName;
        usuario.LastName = formulario.LastName;
        usuario.Email = formulario.Email;
        usuario.PhoneNumber = formulario.PhoneNumber;
        usuario.Address = formulario.Address;
        usuario.Birthday = formulario.Birthday;
        usuario.Role = formulario.Role;

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Route("Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Users
            .Include(u => u.Loans)
            .FirstOrDefaultAsync(u => u.UsuarioId == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    [Route("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Users
            .Include(u => u.Loans)
            .FirstOrDefaultAsync(u => u.UsuarioId == id);

        if (usuario == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (usuario.Loans.Count > 0)
        {
            TempData["Error"] = "No se puede eliminar: el usuario tiene prestamos registrados.";
            return RedirectToAction(nameof(Index));
        }

        _context.Users.Remove(usuario);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Usuario eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private void CargarRoles()
    {
        var roles = new List<string> { "Administrador", "Bibliotecario", "Usuario" };
        ViewBag.Roles = new SelectList(roles);
    }
}
