using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVCPractice.Models;
using MVCPractice.Data;

namespace MVCPractice.Pages.AuthorPages;

public class CreateModel : PageModel
{
    private readonly BibliotecaContext _context;

    public CreateModel(BibliotecaContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Author Author { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Authors.Add(Author);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
