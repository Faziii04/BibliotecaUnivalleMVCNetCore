using System;
using System.Collections.Generic;

namespace MVCPractice.Models;

public partial class BookAuthor
{
    public int AuthorId { get; set; }

    public int LibroId { get; set; }

    public string? Role { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Libro { get; set; } = null!;
}
