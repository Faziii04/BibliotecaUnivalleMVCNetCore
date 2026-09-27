using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Models;
using MVCPractice.Data;

namespace MVCPractice.Pages.AuthorPages;

public class DetailsModel : PageModel
{
    private readonly BibliotecaContext _context;
    public DetailsModel(BibliotecaContext context)
    {
        _context = context;
    }

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
}
