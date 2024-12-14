namespace LibraryApp.Models.DTOs;

public class UserLoanDTO
{
    public int LoanId {  get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public string Status { get; set; }
    public ICollection<LoanBookDTO> Books { get; set; }
}
