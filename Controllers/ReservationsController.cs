using System.Net;
using LibraryApp.Context;
using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;
using LibraryApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers;

[Route("[controller]")]
[ApiController]
public class ReservationsController : Controller
{
    private readonly AppDbContext _context;

    public ReservationsController(AppDbContext context)
    {
        _context = context;
    }

    // pobranie rezerwacji przez pracownika/administratora
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDTO>>> Index()
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator && role != (int)Role.Pracownik)
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        var reservations = await _context.Loans
            .Include(l => l.User)
            .Include(l => l.LoanItems)
            .ThenInclude(li => li.Book)
            .ThenInclude(b => b.Author)
            .Where(l => l.Status == LoanStatus.Zarezerwowane)
            .ToListAsync();

        var reservationDTOs = reservations.Select(l => new LoanDTO
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

        return View(reservationDTOs);
    }

    // dodanie nowej rezerwacji
    [HttpPost]
    public async Task<ActionResult<LoanResponseDTO>> AddReservation(LoanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return Unauthorized();
            }

            var userId = (int)HttpContext.Session.GetInt32("UserId");

            var hasOverdueLoans = await _context.Loans
                .AnyAsync(l => l.UserId == userId && 
                l.Status == LoanStatus.Trwające && 
                l.DateTo < DateOnly.FromDateTime(DateTime.Now));

            if (hasOverdueLoans)
            {
                return BadRequest(new
                {
                    message = "Masz opóźnione wypożyczenia. Nie możesz zarezerwować nowych książek."
                });
            }

            var activeLoansCount = await _context.Loans
                .Where(l => l.UserId == userId &&
                   (l.Status == LoanStatus.Zarezerwowane || l.Status == LoanStatus.Trwające))
                .CountAsync();

            if (activeLoansCount >= 3)
            {
                return BadRequest(new
                {
                    message = "Nie możesz mieć więcej niż trzech aktywnych rezerwacji lub wypożyczeń."
                });
            }

            var books = await _context.Books
                //.Include(b => b.Author)
                .Where(b => model.BookIds.Contains(b.BookId))
                .ToListAsync();

            if (books.Count != model.BookIds.Count)
            {
                var existingBookIds = books.Select(b => b.BookId);
                var missingBookIds = model.BookIds.Except(existingBookIds);
                return NotFound(new
                {
                    message = $"Książki o ID {string.Join(", ", missingBookIds)} nie zostały znalezione."
                });
            }

            var activeStatuses = new[] { LoanStatus.Zarezerwowane, LoanStatus.Trwające };
            var unavailableBooks = books.Where(b =>
            {
                var activeLoansCount = _context.LoanItems
                    .Include(li => li.Loan)
                    .Where(li => li.BookId == b.BookId && activeStatuses.Contains(li.Loan.Status))
                    .Count();
                return (b.Quantity - activeLoansCount) < model.BookIds.Count(id => id == b.BookId);
            }).ToList();

            if (unavailableBooks.Any())
            {
                var messages = unavailableBooks.Select(b => $"Książka {b.Title} jest niedostępna");
                return BadRequest(new { message = messages });
            }

            var loan = new Loan
            {
                UserId = userId,
                LoanItems = new List<LoanItem>()
            };

            foreach (var bookId in model.BookIds)
            {
                loan.LoanItems.Add(new LoanItem
                {
                    BookId = bookId,
                });
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var loanResponse = new LoanResponseDTO
            {
                LoanId = loan.LoanId,
                DateFrom = loan.DateFrom,
                DateTo = loan.DateTo,
                Status = loan.Status.ToString(),
                //Books = loan.LoanItems
                //            .Select(li => new LoanBookDTO
                //            {
                //                BookId = li.BookId,
                //                Title = books.First(b => b.BookId == li.BookId).Title,
                //                Author = new AuthorDTO
                //                {
                //                    AuthorId = books.First(b => b.BookId == li.BookId).Author.AuthorId,
                //                    FirstName = books.First(b => b.BookId == li.BookId).Author.FirstName,
                //                    LastName = books.First(b => b.BookId == li.BookId).Author.LastName
                //                }
                //            })
                //            .ToList()
            };

            //return Ok(loanResponse);
            return StatusCode((int)HttpStatusCode.Created, loanResponse);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new
            {
                message = "Wystąpił błąd podczas tworzenia wypożyczenia.",
                details = ex.Message
            });
        }
    }

    // anulowanie wszystkich przestarzałych rezerwacji przez pracownika/administratora
    [HttpPut("CancelExpiredReservations")]
    public async Task<ActionResult> CancelExpiredReservations()
    {
        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator && role != (int)Role.Pracownik)
        {
            return Unauthorized();
        }

        var reservationValidityPeriod = TimeSpan.FromDays(3);
        var cutoffDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3));

        var expiredReservations = await _context.Loans
            .Where(l => l.Status == LoanStatus.Zarezerwowane && l.DateFrom <= cutoffDate)
            .ToListAsync();

        if (expiredReservations == null || !expiredReservations.Any())
        {
            return Ok(new
            {
                message = "Brak przestarzałych rezerwacji.",
                ids = Array.Empty<int>()
            });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var loan in expiredReservations)
            {
                loan.Status = LoanStatus.Anulowane;
                loan.DateTo = DateOnly.FromDateTime(DateTime.Now);
            }

            _context.Loans.UpdateRange(expiredReservations);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                message = $"{expiredReservations.Count} rezerwacji zostało anulowanych.",
                ids = expiredReservations.Select(r => r.LoanId).ToArray()
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new
            {
                message = "Wystąpił błąd podczas anulowania przestarzałych rezerwacji.",
                details = ex.Message
            });
        }
    }

    // anulowanie rezerwacji przez rezerwującego
    [HttpPut("{id}/CancelReservation")]
    public async Task<ActionResult> CancelReservation(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return Unauthorized();
        }

        var userId = (int)HttpContext.Session.GetInt32("UserId");
        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == id);

        if (loan == null)
        {
            return NotFound(new { message = $"Wypożyczenie o ID {id} nie zostało znalezione." });
        }

        if (loan.UserId != userId)
        {
            return Unauthorized();
        }

        if (loan.Status != LoanStatus.Zarezerwowane)
        {
            return BadRequest(new { message = "Tylko rezerwacje mogą być anulowane." });
        }

        loan.Status = LoanStatus.Anulowane;
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

        return Ok(new { message = "Rezerwacja została anulowana." });
    }

    // rozpoczęcie wypożyczenia przez pracownika/administratora
    [HttpPut("{id}/Start")]
    public async Task<ActionResult> StartLoan(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var role = HttpContext.Session.GetInt32("Role");
        if (role == null | role != (int)Role.Administrator && role != (int)Role.Pracownik)
        {
            return Unauthorized();
        }

        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == id);
        if (loan == null)
        {
            return NotFound(new { message = $"Wypożyczenie o ID {id} nie zostało znalezione." });
        }

        if (loan.Status != LoanStatus.Zarezerwowane)
        {
            return BadRequest(new
            {
                message = "Oznaczyć jako trwające wypożyczenie można jedynie rezerwację."
            });
        }

        loan.Status = LoanStatus.Trwające;
        loan.DateFrom = DateOnly.FromDateTime(DateTime.Now);
        loan.DateTo = loan.DateFrom.AddMonths(1);
        try
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Wystąpił błąd", details = ex.Message });
        }

        return Ok(new { message = "Wypożyczenie zostało rozpoczęte." });
    }
}
