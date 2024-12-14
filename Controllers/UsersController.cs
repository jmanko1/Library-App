using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models.Entities;
using LibraryApp.Repositories.Users;
using System.Text;
using System.Security.Cryptography;
using LibraryApp.Models.ViewModels;
using LibraryApp.Models.DTOs;
using System.Net;

namespace LibraryApp.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Data", "Users");
        }

        return View();
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Data", "Users");
        }

        return View();
    }

    [HttpGet("Data")]
    public async Task<ActionResult<UserDTO>> Data()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Users");
        }

        int userId = (int)HttpContext.Session.GetInt32("UserId");
        var userDTO = await _userRepository.GetUserWithLoansByIdAsync(userId);

        return View(userDTO);
    }

    [HttpGet("Settings")]
    public IActionResult Settings()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Users");
        }

        var userSettingsDTO = new UserSettingsDTO
        {
            Login = HttpContext.Session.GetString("Login"),
            Email = HttpContext.Session.GetString("Email")
        };

        return View(userSettingsDTO);
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Users");
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(await _userRepository.IsUsernameInUse(model.Login))
        {
            return BadRequest(new
            {
                message = "Istnieje już użytkownik o takiej nazwie."
            });
        }

        if(await _userRepository.IsEmailInUse(model.Email))
        {
            return BadRequest(new
            {
                message = "Ten adres email jest już zajęty."
            });
        }

        var user = new User
        {
            Login = model.Login,
            Email = model.Email,
            PasswordHash = HashPassword(model.Password),
            FirstName = model.FirstName,
            LastName = model.LastName,
            Street = model.Street,
            City = model.City
        };

        await _userRepository.AddUserAsync(user);

        return StatusCode((int)HttpStatusCode.Created);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var user = await _userRepository.GetUserByUsernameAsync(model.Login);
        if (user != null && user.PasswordHash == HashPassword(model.Password))
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Login", user.Login);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetInt32("Role", (int) user.Role);

            return Ok();
        }
            
        return BadRequest(new
        {
            message = "Nieprawidłowa nazwa użytkownika lub hasło."
        });
    }

    [HttpPut("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if(HttpContext.Session.GetInt32("UserId") == null)
        {
            return Unauthorized();
        }

        int userId = (int)HttpContext.Session.GetInt32("UserId");
        var user = await _userRepository.GetUserByIdAsync(userId);
        if(user.PasswordHash != HashPassword(model.OldPassword))
        {
            return Unauthorized(new
            {
                message = "Nieprawidłowe hasło."
            });
        }

        user.PasswordHash = HashPassword(model.NewPassword);
        try
        {
            await _userRepository.UpdateUserAsync(user);

            //return Ok(new
            //{
            //    message = "Hasło zostało zmienione."
            //});
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new
            {
                message = "Wystąpił błąd przy zmianie hasła.",
                details = ex.Message
            });
        }
    }

    [HttpPut("ChangeEmail")]
    public async Task<IActionResult> ChangeEmail(ChangeEmailModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return Unauthorized();
        }

        int userId = (int)HttpContext.Session.GetInt32("UserId");

        if(await _userRepository.IsEmailInUse(model.NewEmail))
        {
            return BadRequest(new
            {
                message = "Ten adres email jest już zajęty."
            });
        }

        var user = await _userRepository.GetUserByIdAsync(userId);
        if(user.PasswordHash != HashPassword(model.Password))
        {
            return Unauthorized(new
            {
                message = "Nieprawidłowe hasło."
            });
        }

        user.Email = model.NewEmail;
        try
        {
            await _userRepository.UpdateUserAsync(user);
            HttpContext.Session.SetString("Email", model.NewEmail);

            //return Ok(new
            //{
            //    message = "Adres email został zmieniony."
            //});
            return NoContent();
        }
        catch(DbUpdateException ex)
        {
            return StatusCode(500, new
            {
                message = "Wystąpił błąd przy zmianie adresu email.",
                details = ex.Message
            });
        }
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    //private string Capitalize(string str)
    //{
    //    if (string.IsNullOrEmpty(str))
    //        return str;

    //    return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    //}
}
