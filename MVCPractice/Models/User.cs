using System;
using System.Collections.Generic;

namespace MVCPractice.Models;

public partial class User
{
    public int UsuarioId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly? Birthday { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
