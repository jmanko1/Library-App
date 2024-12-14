using LibraryApp.Models.DTOs;
using LibraryApp.Repositories.Books;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

[Route("[controller]")]
[ApiController]
public class SearchController : Controller
{
    private readonly IBookRepository _bookRepository;

    public SearchController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet("{pattern}")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> Index(string pattern)
    {
        if(pattern == null || pattern == "")
        {
            return BadRequest();
        }

        var books = await _bookRepository.GetBooksByPattern(pattern);

        return View(books);
    }
}
