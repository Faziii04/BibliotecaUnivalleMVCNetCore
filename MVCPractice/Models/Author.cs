using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCPractice.Models;

public partial class Author
{
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public int AuthorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Nickname { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Biography { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
