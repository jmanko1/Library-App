using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.ViewModels;

public class LoanViewModel
{
    [MinLength(1, ErrorMessage = "Musisz wybrać przynajmniej jedną książkę.")]
    [MaxLength(3, ErrorMessage = "Możesz zarezerwować maksymalnie 3 książki jednocześnie.")]
    public List<int> BookIds { get; set; }
}
