using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApp.Models.Entities;

public class Book
{
    [Key]
    public int BookId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public int Quantity { get; set; }

    // Relacje
    [Required]
    public int AuthorId { get; set; }

    [JsonIgnore]
    public Author Author { get; set; }
    
    [JsonIgnore]
    public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
}
