using System.Net;
using LibraryApp.Models.DTOs;
using LibraryApp.Models.Entities;
using LibraryApp.Models.ViewModels;
using LibraryApp.Repositories.Authors;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet("new")]
        public IActionResult AddNewAuthor()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            return View();
        }

        [HttpGet("manage")]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> ManageAuthors()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var authors = await _authorRepository.GetAllAuthors();

            var authorDtos = authors.Select(a => new AuthorDTO
            {
                AuthorId = a.AuthorId,
                FirstName = a.FirstName,
                LastName = a.LastName
            });

            return View(authorDtos);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var authors = await _authorRepository.GetAllAuthors();

            var authorDtos = authors.Select(a => new AuthorDTO
            {
                AuthorId = a.AuthorId,
                FirstName = a.FirstName,
                LastName = a.LastName
            });

            return Ok(authorDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDTO>> GetAuthorById(int id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var author = await _authorRepository.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }

            var authorDto = new AuthorDTO
            {
                AuthorId = author.AuthorId,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            return View(authorDto);
        }

        [HttpGet("search/{pattern}")]
        public async Task<ActionResult<AuthorDTO>> GetAuthorsByPattern(string pattern)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var authors = await _authorRepository.GetAuthorsByPattern(pattern);
            var authorDtos = authors.Select(a => new AuthorDTO
            {
                AuthorId = a.AuthorId,
                FirstName = a.FirstName,
                LastName = a.LastName
            });

            return Ok(authors);
        }

        [HttpPost]
        public async Task<ActionResult> PostAuthor(AuthorViewModel model)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var author = new Author
            {
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            await _authorRepository.AddAuthor(author);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EditAuthor(int id, AuthorViewModel model)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var author = await _authorRepository.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }

            if (author.FirstName == model.FirstName && author.LastName == model.LastName)
            {
                return NoContent();
            }

            var hasActiveLoansOrReservations = author.Books
            .SelectMany(b => b.LoanItems)
            .Any(li => li.Loan != null &&
               (li.Loan.Status == LoanStatus.Zarezerwowane || li.Loan.Status == LoanStatus.Trwające));

            if (hasActiveLoansOrReservations)
            {
                return BadRequest(new { message = "Nie można zaktualizować danych autora, ponieważ przynajmniej jedna z jego książek jest aktualnie zarezerwowana lub wypożyczona." });
            }

            author.FirstName = model.FirstName;
            author.LastName = model.LastName;
            
            await _authorRepository.EditAuthor(author);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role == null | role != (int)Role.Administrator)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var author = await _authorRepository.GetAuthorByIdToUpdateDelete(id);
            if (author == null)
            {
                return NotFound();
            }

            var hasActiveLoansOrReservations = author.Books
            .SelectMany(b => b.LoanItems)
            .Any(li => li.Loan.Status == LoanStatus.Zarezerwowane || li.Loan.Status == LoanStatus.Trwające);

            if (hasActiveLoansOrReservations)
            {
                return BadRequest(new { message = "Nie można usunąć autora, ponieważ przynajmniej jedna z jego książek jest aktualnie zarezerwowana lub wypożyczona." });
            }

            await _authorRepository.DeleteAuthor(author);
            return NoContent();
        }
    }
}
