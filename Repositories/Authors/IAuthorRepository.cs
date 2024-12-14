using LibraryApp.Models.Entities;

namespace LibraryApp.Repositories.Authors;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAuthors();
    Task<Author> GetAuthorById(int id);
    Task<Author> GetAuthorByIdToUpdateDelete(int id);
    Task<IEnumerable<Author>> GetAuthorsByPattern(string pattern);
    Task AddAuthor(Author author);
    Task EditAuthor(Author author);
    Task DeleteAuthor(Author author);
}
