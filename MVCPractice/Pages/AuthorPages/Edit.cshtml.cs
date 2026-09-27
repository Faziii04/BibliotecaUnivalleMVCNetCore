using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Models;
using MVCPractice.Data;

namespace MVCPractice.Pages.AuthorPages;

public class EditModel : PageModel
{
    private readonly BibliotecaContext _context;

    public EditModel(BibliotecaContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Author Author { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? authorid)
    {
        if (authorid is null)
        {
            return NotFound();
        }

        var author = await _context.Authors.FirstOrDefaultAsync(m => m.AuthorId == authorid);
        if (author is null)
        {
            return NotFound();
        }
        Author = author;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Author).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists(Author.AuthorId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool AuthorExists(int authorid)
    {
        return _context.Authors.Any(e => e.AuthorId == authorid);
    }
}
