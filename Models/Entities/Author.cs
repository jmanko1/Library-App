using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.Entities;

public class Author
{
    [Key]
    public int AuthorId { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    // Relacje
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
