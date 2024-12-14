using LibraryApp.Models.Entities;

namespace LibraryApp.Models.DTOs;

public class LoanUserDTO
{
    public int UserId { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Role { get; set; }
    public DateOnly SignupDate { get; set; }
}
