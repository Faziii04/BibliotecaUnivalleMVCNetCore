using System.ComponentModel.DataAnnotations;

namespace MVCPractice.ViewModels;

public class LoanFormModel
{
    public int BorrowId { get; set; }

    [Required(ErrorMessage = "Seleccione un libro.")]
    [Display(Name = "Libro")]
    public int LibroId { get; set; }

    [Required(ErrorMessage = "Seleccione un usuario.")]
    [Display(Name = "Usuario")]
    public int UsuarioId { get; set; }

    [Display(Name = "Fecha del prestamo")]
    [DataType(DataType.Date)]
    public DateOnly LoanDate { get; set; }

    [Display(Name = "Dias de prestamo")]
    [Range(1, 365, ErrorMessage = "Indique entre 1 y 365 dias.")]
    public int? LoanDurationDays { get; set; }

    [Display(Name = "Devuelto")]
    public bool IsReturned { get; set; }
}
