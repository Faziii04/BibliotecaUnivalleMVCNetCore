using MVCPractice.Models;

namespace MVCPractice.ViewModels;

public class DashboardViewModel
{
    public User Usuario { get; set; } = null!;

    public string Rol { get; set; } = string.Empty;

    public List<Loan> Prestamos { get; set; } = new();

    public List<Book> Catalogo { get; set; } = new();

    public int CantidadLibros { get; set; }

    public int EjemplaresDisponibles { get; set; }

    public int PrestamosActivos { get; set; }

    public int PrestamosDevueltos { get; set; }

    public int CantidadUsuarios { get; set; }
}
