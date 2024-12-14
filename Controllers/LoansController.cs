using System.Net;
using LibraryApp.Context;
using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers;

[Route("[controller]")]
[ApiController]
public class LoansController : Controller
{
    private readonly AppDbContext _context;

    public LoansController(AppDbContext context)
    {
        _context = context;
    }

    // Pobranie trwających wypożyczeń przez pracownika/administratora
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDTO>>> Index()
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator && role != (int)Role.Pracownik)
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        var loans = await _context.Loans
            .Include(l => l.User)
            .Include(l => l.LoanItems)
            .ThenInclude(li => li.Book)
            .ThenInclude(b => b.Author)
            .Where(l => l.Status == LoanStatus.Trwające)
            .ToListAsync();

        var loanDTOs = loans.Select(l => new LoanDTO
        {
            LoanId = l.LoanId,
            User = new LoanUserDTO
            {
                UserId = l.User.UserId,
                Login = l.User.Login,
                Email = l.User.Email,
                FirstName = l.User.FirstName,
                LastName = l.User.LastName,
                Street = l.User.Street,
                City = l.User.City,
                Role = l.User.Role.ToString(),
                SignupDate = l.User.SignupDate
            },
            DateFrom = l.DateFrom,
            DateTo = l.DateTo,
            Status = l.Status.ToString(),
            Books = l.LoanItems.Select(li => new LoanBookDTO
            {
                BookId = li.BookId,
                Title = li.Book.Title,
                Author = new AuthorDTO
                {
                    AuthorId = li.Book.AuthorId,
                    FirstName = li.Book.Author.FirstName,
                    LastName = li.Book.Author.LastName
                }
            }).ToList()
        });

        return View(loanDTOs);
    }

    // wydłużenie wypożyczenia o miesiąc przez wypożyczającego
    [HttpPut("{id}/Extend")]
    public async Task<ActionResult> ExtendLoan(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return Unauthorized();
        }

        var userId = (int) HttpContext.Session.GetInt32("UserId");
        var loan = await _context.Loans
            .Include(l => l.LoanItems)
                .ThenInclude(li => li.Book)
            .FirstOrDefaultAsync(l => l.LoanId == id);

        if(loan == null)
        {
            return NotFound(new { message = $"Wypożyczenie o ID {id} nie zostało znalezione." });
        }

        if(loan.UserId != userId)
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        if(loan.Status != LoanStatus.Trwające && loan.Status != LoanStatus.Zarezerwowane)
        {
            return BadRequest(new {message = "Tylko aktywne wypożyczenia mogą zostać wydłużone."});
        }

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        if(currentDate > loan.DateTo)
        {
            return BadRequest(new { message = "Nie można wydłużyć wypożyczenia, ponieważ termin zwrotu już minął" });
        }

        var totalDuration = (loan.DateTo.ToDateTime(TimeOnly.MinValue) - loan.DateFrom.ToDateTime(TimeOnly.MinValue)).TotalDays;
        if (totalDuration >= 93)
        {
            return BadRequest(new { message = "Wypożyczenie nie może trwać dłużej niż 3 miesiące." });
        }

        var activeStatuses = new[] { LoanStatus.Zarezerwowane, LoanStatus.Trwające };
        foreach(var loanItem in loan.LoanItems)
        {
            var book = loanItem.Book;
            var activeLoanCount = await _context.LoanItems
                .Where(li => li.BookId == book.BookId && activeStatuses.Contains(li.Loan.Status))
                .CountAsync();
            int availableCopies = book.Quantity - activeLoanCount;

            if(availableCopies < 1)
            {
                return BadRequest(new { message = $"Książka '{book.Title}' nie ma dostępnych egzemplarzy, by móc wydłużyć wypożyczenie." });
            }
        }

        loan.DateTo = loan.DateTo.AddMonths(1);
        var newDuration = (loan.DateTo.ToDateTime(TimeOnly.MinValue) - loan.DateFrom.ToDateTime(TimeOnly.MinValue)).TotalDays;
        if(newDuration > 93)
        {
            return BadRequest(new { message = "Wypożyczenie nie może trwać dłużej niż 3 miesiące." });
        }

        try
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Wystąpił błąd", details = ex.Message });
        }

        return Ok(new { message = $"Wypożyczenie zostało wydłużone do {loan.DateTo}" });
    }

    // zakończenie wypożyczenia przez pracownika/administratora
    [HttpPut("{id}/Finish")]
    public async Task<ActionResult> FinishLoan(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator && role != (int)Role.Pracownik)
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == id);
        if (loan == null)
        {
            return NotFound(new { message = $"Wypożyczenie o ID {id} nie zostało znalezione." });
        }

        if (loan.Status != LoanStatus.Trwające)
        {
            return BadRequest(new
            {
                message = "Zakończyć można jedynie trwające wypożyczenie."
            });
        }

        loan.Status = LoanStatus.Zakończone;
        loan.DateTo = DateOnly.FromDateTime(DateTime.Now);
        try
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Wystąpił błąd", details = ex.Message });
        }

        return Ok(new { message = "Wypożyczenie zostało zakończone." });
    }
}
