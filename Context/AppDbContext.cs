using LibraryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LibraryApp.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanItem> LoanItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relacja książka - autor
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacja wypożyczenie - użytkownik
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.User)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja pozycja wypożyczenia - wypożyczenie
        modelBuilder.Entity<LoanItem>()
            .HasOne(li => li.Loan)
            .WithMany(l => l.LoanItems)
            .HasForeignKey(li => li.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja pozycja wypożyczenia - książka
        modelBuilder.Entity<LoanItem>()
            .HasOne(li => li.Book)
            .WithMany(b => b.LoanItems)
            .HasForeignKey(li => li.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unikalność pól Login i Email w użytkowniku
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();



        // Definicja konwertera dla DateOnly
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        // Zastosowanie konwertera do właściwości DateFrom i DateTo w Loan
        modelBuilder.Entity<Loan>()
            .Property(l => l.DateFrom)
            .HasConversion(dateOnlyConverter);

        modelBuilder.Entity<Loan>()
            .Property(l => l.DateTo)
            .HasConversion(dateOnlyConverter);

        // Zastosowanie konwertera do właściwości SignupDate w User
        modelBuilder.Entity<User>()
            .Property(u => u.SignupDate)
            .HasConversion(dateOnlyConverter);
    }
}
