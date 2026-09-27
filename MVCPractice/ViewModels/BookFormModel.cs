using System.ComponentModel.DataAnnotations;

namespace MVCPractice.ViewModels;

public class BookFormModel
{
    public int LibroId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Precio")]
    [Range(0, 99999999, ErrorMessage = "Ingrese un precio valido.")]
    public decimal? Price { get; set; }

    [Display(Name = "Paginas")]
    public int? Lenght { get; set; }

    [Display(Name = "Formato")]
    public string? Format { get; set; }

    [Display(Name = "Categoria")]
    public string? Category { get; set; }

    [Display(Name = "Edad")]
    public string? AgeGroup { get; set; }

    [Display(Name = "Stock")]
    [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Display(Name = "Fecha de publicacion")]
    [DataType(DataType.Date)]
    public DateOnly? PublishingDate { get; set; }

    [Display(Name = "Autores")]
    public List<int> SelectedAuthorIds { get; set; } = new();
}
