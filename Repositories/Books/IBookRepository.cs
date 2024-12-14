using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;

namespace LibraryApp.Repositories.Books;

public interface IBookRepository
{
    Task<IEnumerable<BookDTO>> GetAllBooks();
    Task<BookDTO> GetBookById(int id);
    Task PostBook(Book book);
    Task<IEnumerable<BookDTO>> GetBooksByPattern(string pattern);

    Task UpdateBook(Book book);
    Task<Book> GetBookEntityById(int id);

    Task DeleteBook(Book book);
}
