using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    [MinLength(4, ErrorMessage = "Nazwa użytkownika musi mieć co najmniej 4 znaki.")]
    [MaxLength(50)]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Login może zawierać tylko litery i cyfry.")]
    public string Login { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Nieprawidłowy adres email.")]
    public string Email { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

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
}
