using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Data;
using MVCPractice.Models;
using MVCPractice.ViewModels;

namespace MVCPractice.Controllers;

[Authorize]
[Route("[controller]")]
public class BooksController : Controller
{
    private readonly BibliotecaContext _context;

    public BooksController(BibliotecaContext context)
    {
        _context = context;
    }

    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index(string? buscar)
    {
        var consulta = _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .AsQueryable();

        if (!User.IsInRole("Administrador") && !User.IsInRole("Bibliotecario"))
        {
            consulta = consulta.Where(b => b.Stock > 0);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(b => b.Name.Contains(buscar));
        }

        ViewBag.Buscar = buscar;

        return View(await consulta.OrderBy(b => b.Name).ToListAsync());
    }

    [Route("Details")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.LibroId == id);

        if (libro == null)
        {
            return NotFound();
        }

        return View(libro);
    }

    [Authorize(Roles = "Administrador,Bibliotecario")]
    [Route("Create")]
    public async Task<IActionResult> Create()
    {
        await CargarAutores();
        return View(new BookFormModel());
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Bibliotecario")]
    [Route("Create")]
    public async Task<IActionResult> Create(BookFormModel formulario)
    {
        if (!ModelState.IsValid)
        {
            await CargarAutores(formulario.SelectedAuthorIds);
            return View(formulario);
        }

        var libro = new Book
        {
            Name = formulario.Name,
            Price = formulario.Price,
            Lenght = formulario.Lenght,
            Format = formulario.Format,
            Category = formulario.Category,
            AgeGroup = formulario.AgeGroup,
            Stock = formulario.Stock,
            PublishingDate = formulario.PublishingDate
        };

        foreach (var autorId in formulario.SelectedAuthorIds)
        {
            libro.BookAuthors.Add(new BookAuthor { AuthorId = autorId, Role = "Autor" });
        }

        _context.Books.Add(libro);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Libro registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador,Bibliotecario")]
    [Route("Edit")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _context.Books
            .Include(b => b.BookAuthors)
            .FirstOrDefaultAsync(b => b.LibroId == id);

        if (libro == null)
        {
            return NotFound();
        }

        await CargarAutores(libro.BookAuthors.Select(ba => ba.AuthorId));

        return View(new BookFormModel
        {
            LibroId = libro.LibroId,
            Name = libro.Name,
            Price = libro.Price,
            Lenght = libro.Lenght,
            Format = libro.Format,
            Category = libro.Category,
            AgeGroup = libro.AgeGroup,
            Stock = libro.Stock,
            PublishingDate = libro.PublishingDate,
            SelectedAuthorIds = libro.BookAuthors.Select(ba => ba.AuthorId).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Bibliotecario")]
    [Route("Edit")]
    public async Task<IActionResult> Edit(int id, BookFormModel formulario)
    {
        if (id != formulario.LibroId)
        {
            return NotFound();
        }

        var libro = await _context.Books
            .Include(b => b.BookAuthors)
            .FirstOrDefaultAsync(b => b.LibroId == id);

        if (libro == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await CargarAutores(formulario.SelectedAuthorIds);
            return View(formulario);
        }

        libro.Name = formulario.Name;
        libro.Price = formulario.Price;
        libro.Lenght = formulario.Lenght;
        libro.Format = formulario.Format;
        libro.Category = formulario.Category;
        libro.AgeGroup = formulario.AgeGroup;
        libro.Stock = formulario.Stock;
        libro.PublishingDate = formulario.PublishingDate;

        _context.BookAuthors.RemoveRange(libro.BookAuthors);
        libro.BookAuthors.Clear();

        foreach (var autorId in formulario.SelectedAuthorIds)
        {
            libro.BookAuthors.Add(new BookAuthor { AuthorId = autorId, LibroId = libro.LibroId, Role = "Autor" });
        }

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Libro actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador,Bibliotecario")]
    [Route("Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.LibroId == id);

        if (libro == null)
        {
            return NotFound();
        }

        return View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrador,Bibliotecario")]
    [Route("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var libro = await _context.Books
            .Include(b => b.BookAuthors)
            .Include(b => b.Loans)
            .FirstOrDefaultAsync(b => b.LibroId == id);

        if (libro == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (libro.Loans.Count > 0)
        {
            TempData["Error"] = "No se puede eliminar: el libro tiene prestamos registrados.";
            return RedirectToAction(nameof(Index));
        }

        _context.BookAuthors.RemoveRange(libro.BookAuthors);
        _context.Books.Remove(libro);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Libro eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarAutores(IEnumerable<int>? seleccionados = null)
    {
        var autores = await _context.Authors.OrderBy(a => a.LastName).ToListAsync();
        ViewBag.Autores = new MultiSelectList(autores, "AuthorId", "FullName", seleccionados);
    }
}
