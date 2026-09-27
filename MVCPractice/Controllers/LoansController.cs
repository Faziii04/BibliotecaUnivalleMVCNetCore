using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Data;
using MVCPractice.Models;
using MVCPractice.ViewModels;

namespace MVCPractice.Controllers;

[Authorize(Roles = "Administrador,Bibliotecario")]
[Route("[controller]")]
public class LoansController : Controller
{
    private readonly BibliotecaContext _context;

    public LoansController(BibliotecaContext context)
    {
        _context = context;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index(string? buscar)
    {
        var consulta = _context.Loans
            .Include(l => l.Libro)
            .Include(l => l.Usuario)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(l => l.Libro.Name.Contains(buscar) || l.Usuario.Email.Contains(buscar));
        }

        ViewBag.Buscar = buscar;

        return View(await consulta.OrderByDescending(l => l.LoanDate).ToListAsync());
    }

    [Route("Details")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var prestamo = await _context.Loans
            .Include(l => l.Libro)
            .Include(l => l.Usuario)
            .FirstOrDefaultAsync(l => l.BorrowId == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        return View(prestamo);
    }

    [Route("Create")]
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View(new LoanFormModel
        {
            LoanDate = DateOnly.FromDateTime(DateTime.Today),
            LoanDurationDays = 7
        });
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(LoanFormModel formulario)
    {
        var libro = await _context.Books.FirstOrDefaultAsync(b => b.LibroId == formulario.LibroId);

        if (libro == null)
        {
            ModelState.AddModelError("LibroId", "El libro seleccionado no existe.");
        }
        else if (libro.Stock <= 0)
        {
            ModelState.AddModelError("LibroId", "No hay ejemplares disponibles de este libro.");
        }

        if (!ModelState.IsValid)
        {
            await CargarListas(formulario.LibroId, formulario.UsuarioId);
            return View(formulario);
        }

        libro!.Stock = libro.Stock - 1;

        _context.Loans.Add(new Loan
        {
            LibroId = formulario.LibroId,
            UsuarioId = formulario.UsuarioId,
            LoanDate = formulario.LoanDate,
            LoanTime = TimeOnly.FromDateTime(DateTime.Now),
            LoanDurationDays = formulario.LoanDurationDays,
            IsReturned = false
        });

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Prestamo registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Route("Edit")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var prestamo = await _context.Loans.FirstOrDefaultAsync(l => l.BorrowId == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        await CargarListas(prestamo.LibroId, prestamo.UsuarioId);

        return View(new LoanFormModel
        {
            BorrowId = prestamo.BorrowId,
            LibroId = prestamo.LibroId,
            UsuarioId = prestamo.UsuarioId,
            LoanDate = prestamo.LoanDate,
            LoanDurationDays = prestamo.LoanDurationDays,
            IsReturned = prestamo.IsReturned
        });
    }

    [HttpPost]
    [Route("Edit")]
    public async Task<IActionResult> Edit(int id, LoanFormModel formulario)
    {
        if (id != formulario.BorrowId)
        {
            return NotFound();
        }

        var prestamo = await _context.Loans.FirstOrDefaultAsync(l => l.BorrowId == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await CargarListas(formulario.LibroId, formulario.UsuarioId);
            return View(formulario);
        }

        var libro = await _context.Books.FirstOrDefaultAsync(b => b.LibroId == prestamo.LibroId);

        if (prestamo.IsReturned != formulario.IsReturned)
        {
            if (formulario.IsReturned)
            {
                prestamo.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
                prestamo.ReturnTime = TimeOnly.FromDateTime(DateTime.Now);

                if (libro != null)
                {
                    libro.Stock = libro.Stock + 1;
                }
            }
            else
            {
                prestamo.ReturnDate = null;
                prestamo.ReturnTime = null;

                if (libro != null)
                {
                    libro.Stock = libro.Stock - 1;
                }
            }

            prestamo.IsReturned = formulario.IsReturned;
        }

        prestamo.LoanDate = formulario.LoanDate;
        prestamo.LoanDurationDays = formulario.LoanDurationDays;

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Prestamo actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Route("Return")]
    public async Task<IActionResult> Return(int id)
    {
        var prestamo = await _context.Loans
            .Include(l => l.Libro)
            .FirstOrDefaultAsync(l => l.BorrowId == id);

        if (prestamo != null && !prestamo.IsReturned)
        {
            prestamo.IsReturned = true;
            prestamo.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
            prestamo.ReturnTime = TimeOnly.FromDateTime(DateTime.Now);
            prestamo.Libro.Stock = prestamo.Libro.Stock + 1;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Devolucion registrada.";
        }

        return RedirectToAction(nameof(Index));
    }

    [Route("Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var prestamo = await _context.Loans
            .Include(l => l.Libro)
            .Include(l => l.Usuario)
            .FirstOrDefaultAsync(l => l.BorrowId == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        return View(prestamo);
    }

    [HttpPost, ActionName("Delete")]
    [Route("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var prestamo = await _context.Loans
            .Include(l => l.Libro)
            .FirstOrDefaultAsync(l => l.BorrowId == id);

        if (prestamo == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!prestamo.IsReturned)
        {
            prestamo.Libro.Stock = prestamo.Libro.Stock + 1;
        }

        _context.Loans.Remove(prestamo);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Prestamo eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListas(int? libroId = null, int? usuarioId = null)
    {
        var libros = await _context.Books.OrderBy(b => b.Name).ToListAsync();
        var usuarios = await _context.Users.OrderBy(u => u.Email).ToListAsync();

        ViewBag.Libros = new SelectList(libros, "LibroId", "Name", libroId);
        ViewBag.Usuarios = new SelectList(usuarios, "UsuarioId", "Email", usuarioId);
    }
}
