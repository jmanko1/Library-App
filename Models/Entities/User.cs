using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Login { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string Street { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; }

    [Required]
    public Role Role { get; set; } = Role.Czytelnik;

    [Required]
    public DateOnly SignupDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    // Relacje
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}

public enum Role
{ 
    Administrator = 1,
    Pracownik = 2,
    Czytelnik = 3
}
