using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApp.Models.Entities;

public class Loan
{
    [Key]
    public int LoanId { get; set; }

    // Relacje
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    public DateOnly DateFrom { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Required]
    public DateOnly DateTo { get; set; } = DateOnly.FromDateTime(DateTime.Now).AddDays(2);

    [Required]
    public LoanStatus Status { get; set; } = LoanStatus.Zarezerwowane;

    // Relacje
    public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
}
public enum LoanStatus
{
    Zarezerwowane = 1,
    Anulowane = 2,
    Trwające = 3,
    Zakończone = 4
}
