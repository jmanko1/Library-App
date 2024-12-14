using System.Net;
using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;
using LibraryApp.Models.ViewModels;
using LibraryApp.Repositories.Books;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

[Route("[controller]")]
[ApiController]
public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks()
    {
        var books = await _bookRepository.GetAllBooks();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBookById(int id)
    {
        var book = await _bookRepository.GetBookById(id);
        
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpGet("new")]
    public IActionResult AddNewBook()
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                message = "Brak uprawnień"
            });
        }

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> PostBook(BookViewModel model)
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                message = "Brak uprawnień"
            });
        }

        var book = new Book
        {
            Title = model.Title,
            Quantity = model.Quantity,
            AuthorId = model.AuthorId
        };

        await _bookRepository.PostBook(book);
        return StatusCode((int)HttpStatusCode.Created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBook(int id, BookViewModel model)
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                message = "Brak uprawnień"
            });
        }

        var book = await _bookRepository.GetBookEntityById(id);
        if(book == null)
        {
            return NotFound(new { message = $"Książka o ID {id} nie istnieje." });
        }

        if(model.Title == book.Title && model.AuthorId == book.AuthorId && model.Quantity == book.Quantity)
        {
            return NoContent();
        }

        if (model.Title != book.Title || model.AuthorId != book.AuthorId)
        {
            var hasActiveLoansReservations = book.LoanItems
                .Any(li => li.Loan.Status == LoanStatus.Trwające || li.Loan.Status == LoanStatus.Zarezerwowane);

            if (hasActiveLoansReservations)
            {
                return BadRequest(new { message = "Nie można edytować tytułu lub autora książki, która jest wypożyczona lub zarezerwowana." });
            }
        }

        book.Title = model.Title;
        book.Quantity = model.Quantity;
        book.AuthorId = model.AuthorId;

        await _bookRepository.UpdateBook(book);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new
            {
                message = "Brak uprawnień"
            });
        }

        var book = await _bookRepository.GetBookEntityById(id);
        if (book == null)
        {
            return NotFound(new { message = $"Książka o ID {id} nie istnieje." });
        }

        var hasActiveLoansReservations = book.LoanItems
                .Any(li => li.Loan.Status == LoanStatus.Trwające || li.Loan.Status == LoanStatus.Zarezerwowane);

        if (hasActiveLoansReservations)
        {
            return BadRequest(new { message = "Nie można usunąć książki, która jest wypożyczona lub zarezerwowana." });
        }

        await _bookRepository.DeleteBook(book);
        return NoContent();
    }
}
