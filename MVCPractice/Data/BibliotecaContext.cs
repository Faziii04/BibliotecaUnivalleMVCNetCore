using System;
using System.Collections.Generic;
using MVCPractice.Models;
using Microsoft.EntityFrameworkCore;

namespace MVCPractice.Data;

public partial class BibliotecaContext : DbContext
{
    public BibliotecaContext()
    {
    }

    public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__86516BCF0957BE17");

            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Biography).HasColumnName("biography");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Nickname)
                .HasMaxLength(100)
                .HasColumnName("nickname");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.LibroId).HasName("PK__Books__5BB6A2FC8C8F0191");

            entity.Property(e => e.LibroId).HasColumnName("libro_id");
            entity.Property(e => e.AgeGroup)
                .HasMaxLength(50)
                .HasColumnName("age_group");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Format)
                .HasMaxLength(50)
                .HasColumnName("format");
            entity.Property(e => e.Lenght).HasColumnName("lenght");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PublishingDate).HasColumnName("publishing_date");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.AuthorId, e.LibroId }).HasName("PK__Book_Aut__53EA01E0A5C5586F");

            entity.ToTable("Book_Authors");

            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.LibroId).HasColumnName("libro_id");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_WrittenBy_Author");

            entity.HasOne(d => d.Libro).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.LibroId)
                .HasConstraintName("FK_WrittenBy_Libro");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.BorrowId).HasName("PK__Loans__262B57A01A20F5AB");

            entity.Property(e => e.BorrowId).HasColumnName("borrow_id");
            entity.Property(e => e.IsReturned).HasColumnName("is_returned");
            entity.Property(e => e.LibroId).HasColumnName("libro_id");
            entity.Property(e => e.LoanDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("loan_date");
            entity.Property(e => e.LoanDurationDays).HasColumnName("loan_duration_days");
            entity.Property(e => e.LoanTime)
                .HasDefaultValueSql("(CONVERT([time],getdate()))")
                .HasColumnName("loan_time");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.ReturnTime).HasColumnName("return_time");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Libro).WithMany(p => p.Loans)
                .HasForeignKey(d => d.LibroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Borrows_Libro");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Loans)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Borrows_Usuario");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Users__2ED7D2AFB5472C66");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616458A67A86").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
