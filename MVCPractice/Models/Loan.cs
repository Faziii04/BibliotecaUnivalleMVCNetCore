using System;
using System.Collections.Generic;

namespace MVCPractice.Models;

public partial class Loan
{
    public int BorrowId { get; set; }

    public int LibroId { get; set; }

    public int UsuarioId { get; set; }

    public DateOnly LoanDate { get; set; }

    public TimeOnly LoanTime { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public TimeOnly? ReturnTime { get; set; }

    public int? LoanDurationDays { get; set; }

    public bool IsReturned { get; set; }

    public virtual Book Libro { get; set; } = null!;

    public virtual User Usuario { get; set; } = null!;
}
