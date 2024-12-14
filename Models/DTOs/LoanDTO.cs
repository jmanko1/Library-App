namespace LibraryApp.Models.DTOs;

public class LoanDTO
{
    public int LoanId { get; set; }
    public LoanUserDTO User { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public string Status { get; set; }
    public ICollection<LoanBookDTO> Books { get; set; }
}
