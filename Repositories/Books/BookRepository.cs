using LibraryApp.Context;
using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories.Books;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    
    public BookRepository(AppDbContext context) 
    {
        _context = context;
    }

    public async Task DeleteBook(Book book)
    {
        var loanItems = await _context.LoanItems.Where(li => li.BookId == book.BookId).ToListAsync();
        _context.LoanItems.RemoveRange(loanItems);
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookDTO>> GetAllBooks()
    {
        var books = await _context.Books
                                  .Include(b => b.Author)
                                  .Select(b => new BookDTO
                                  {
                                      BookId = b.BookId,
                                      Title = b.Title,
                                      Author = new AuthorDTO
                                      {
                                          AuthorId = b.Author.AuthorId,
                                          FirstName = b.Author.FirstName,
                                          LastName = b.Author.LastName
                                      },
                                      Quantity = b.Quantity,
                                      AvailableCopies = b.Quantity - _context.LoanItems
                                            .Where
                                            (
                                                li => li.BookId == b.BookId &&
                                                (li.Loan.Status == LoanStatus.Trwające || li.Loan.Status == LoanStatus.Zarezerwowane)
                                            )
                                            .Count()
                                  })
                                  .ToListAsync();

        return books;
    }

    public async Task<BookDTO> GetBookById(int id)
    {
        var book = await _context.Books
                                  .Include(b => b.Author)
                                  .Select(b => new BookDTO
                                  {
                                      BookId = b.BookId,
                                      Title = b.Title,
                                      Author = new AuthorDTO
                                      {
                                          AuthorId = b.Author.AuthorId,
                                          FirstName = b.Author.FirstName,
                                          LastName = b.Author.LastName
                                      },
                                      Quantity = b.Quantity,
                                      AvailableCopies = b.Quantity - _context.LoanItems
                                            .Where
                                            (
                                                li => li.BookId == b.BookId &&
                                                (li.Loan.Status == LoanStatus.Trwające || li.Loan.Status == LoanStatus.Zarezerwowane)
                                            )
                                            .Count()
                                  })
                                  .FirstOrDefaultAsync(b => b.BookId == id);

        return book;
    }

    public async Task<Book> GetBookEntityById(int id)
    {
        return await _context.Books
            .Include(b => b.LoanItems)
            .ThenInclude(li => li.Loan)
            .FirstOrDefaultAsync(b => b.BookId == id);
    }

    public async Task<IEnumerable<BookDTO>> GetBooksByPattern(string pattern)
    {
        var books = await _context.Books
                                  .Include(b => b.Author)
                                  .Select(b => new BookDTO
                                  {
                                      BookId = b.BookId,
                                      Title = b.Title,
                                      Author = new AuthorDTO
                                      {
                                          AuthorId = b.Author.AuthorId,
                                          FirstName = b.Author.FirstName,
                                          LastName = b.Author.LastName
                                      },
                                      //Quantity = b.Quantity,
                                      AvailableCopies = b.Quantity - _context.LoanItems
                                            .Where
                                            (
                                                li => li.BookId == b.BookId &&
                                                (li.Loan.Status == LoanStatus.Trwające || li.Loan.Status == LoanStatus.Zarezerwowane)
                                            )
                                            .Count()
                                  })
                                  .Where(b => b.Title.Contains(pattern) || (b.Author.FirstName + " " + b.Author.LastName).Contains(pattern))
                                  .ToListAsync();

        return books;
    }

    public async Task PostBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBook(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }
}
