using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Models;
using MVCPractice.Data;

namespace MVCPractice.Pages.AuthorPages;

public class DeleteModel : PageModel
{
    private readonly BibliotecaContext _context;

    public DeleteModel(BibliotecaContext context)
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
        else
        {
            Author = author;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? authorid)
    {
        if (authorid is null)
        {
            return NotFound();
        }

        var author = await _context.Authors.FindAsync(authorid);
        if (author != null)
        {
            Author = author;
            _context.Authors.Remove(Author);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
