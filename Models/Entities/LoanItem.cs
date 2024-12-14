using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApp.Models.Entities;

public class LoanItem
{
    [Key]
    public int LoanItemId { get; set; }

    // Relacje
    [Required]
    public int LoanId { get; set; }

    [JsonIgnore]
    public Loan Loan { get; set; }

    [Required]
    public int BookId { get; set; }
    public Book Book { get; set; }
}
