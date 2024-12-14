using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.ViewModels;

public class ChangeEmailModel
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
