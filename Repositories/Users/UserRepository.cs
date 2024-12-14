using LibraryApp.Context;
using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetUserByUsernameAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<UserDTO> GetUserWithLoansByIdAsync(int id)
    {
        var userDTO = await _context.Users
                                 .Select(u => new UserDTO
                                 {
                                     UserId = u.UserId,
                                     Login = u.Login,
                                     Email = u.Email,
                                     FirstName = u.FirstName,
                                     LastName = u.LastName,
                                     Street = u.Street,
                                     City = u.City,
                                     Role = u.Role.ToString(),
                                     SignupDate = u.SignupDate,
                                     Loans = u.Loans
                                              .Select(l => new UserLoanDTO
                                              {
                                                  LoanId = l.LoanId,
                                                  DateFrom = l.DateFrom,
                                                  DateTo = l.DateTo,
                                                  Status = l.Status.ToString(),
                                                  Books = l.LoanItems
                                                           .Select(li => new LoanBookDTO
                                                           {
                                                               BookId = li.Book.BookId,
                                                               Title = li.Book.Title,
                                                               Author = new AuthorDTO
                                                               {
                                                                   AuthorId = li.Book.Author.AuthorId,
                                                                   FirstName = li.Book.Author.FirstName,
                                                                   LastName = li.Book.Author.LastName
                                                               }
                                                           })
                                                           .ToList()
                                              })
                                              .ToList()
                                 })
                                 .FirstOrDefaultAsync(u => u.UserId == id);

        return userDTO;
    }

    public async Task<bool> IsUsernameInUse(string username)
    {
        return await _context.Users.AnyAsync(u => u.Login == username);
    }

    public async Task<bool> IsEmailInUse(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
