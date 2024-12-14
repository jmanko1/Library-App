namespace LibraryApp.Models.DTOs;

public class LoanBookDTO
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public AuthorDTO Author { get; set; }
}
