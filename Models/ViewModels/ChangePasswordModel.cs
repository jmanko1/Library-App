using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.ViewModels;

public class ChangePasswordModel
{
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Hasła nie są identyczne.")]
    public string ConfirmNewPassword { get; set; }
}
