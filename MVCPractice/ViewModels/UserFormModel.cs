using System.ComponentModel.DataAnnotations;

namespace MVCPractice.ViewModels;

public class UserFormModel
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombres")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellidos")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo valido.")]
    [Display(Name = "Correo")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefono")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Direccion")]
    [StringLength(255)]
    public string? Address { get; set; }

    [Display(Name = "Fecha de nacimiento")]
    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [Display(Name = "Rol")]
    public string Role { get; set; } = string.Empty;
}
