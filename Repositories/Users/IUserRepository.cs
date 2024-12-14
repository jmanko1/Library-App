using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;

namespace LibraryApp.Repositories.Users;

public interface IUserRepository
{
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> GetUserByIdAsync(int id);
    Task UpdateUserAsync(User user);
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task<UserDTO> GetUserWithLoansByIdAsync(int id);

    Task<bool> IsUsernameInUse(string username);
    Task<bool> IsEmailInUse(string email);
}
