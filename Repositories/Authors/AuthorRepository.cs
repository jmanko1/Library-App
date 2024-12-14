using LibraryApp.Context;
using LibraryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories.Authors;

public class AuthorRepository : IAuthorRepository
{
    private readonly AppDbContext _context;

    public AuthorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAuthor(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAuthor(Author author)
    {
        foreach (var book in author.Books)
        {
            var loanItemsToRemove = _context.LoanItems.Where(li => li.BookId == book.BookId);
            _context.LoanItems.RemoveRange(loanItemsToRemove);
        }

        _context.Books.RemoveRange(author.Books);
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
    }

    public async Task EditAuthor(Author author)
    {
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Author>> GetAllAuthors()
    {
        var authors = await _context.Authors.ToListAsync();
        
        return authors;
    }

    public async Task<Author> GetAuthorById(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == id);

        return author;
    }

    public async Task<Author> GetAuthorByIdToUpdateDelete(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.LoanItems)
            .ThenInclude(li => li.Loan)
            .FirstOrDefaultAsync(a => a.AuthorId == id);

        return author;
    }

    public async Task<IEnumerable<Author>> GetAuthorsByPattern(string pattern)
    {
        var authors = await _context.Authors
            .Where(a => (a.FirstName + " " + a.LastName).Contains(pattern))
            .ToListAsync();

        return authors;
    }
}
