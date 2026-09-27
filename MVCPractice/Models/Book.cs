using System;
using System.Collections.Generic;

namespace MVCPractice.Models;

public partial class Book
{
    public int LibroId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Price { get; set; }

    public int? Lenght { get; set; }

    public string? Format { get; set; }

    public string? Category { get; set; }

    public string? AgeGroup { get; set; }

    public int Stock { get; set; }

    public DateOnly? PublishingDate { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
