using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Models;
using MVCPractice.Data;

namespace MVCPractice.Pages.AuthorPages;

public class IndexModel : PageModel
{
    private readonly BibliotecaContext _context;

    public IndexModel(BibliotecaContext context)
    {
        _context = context;
    }

    public IList<Author> Author { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Author = await _context.Authors.ToListAsync();
    }
}
