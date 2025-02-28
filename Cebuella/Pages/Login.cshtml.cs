using System.Security.Claims;
using Cebuella;
using Cebuella.Models;
using Cebulla;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cebulla.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly AppDbContext context;

    public LoginModel(AppDbContext context)
    {
        this.context = context;
        context.EnsureAdminExists();
    }

    [BindProperty] public required InputModel Input { get; set; }

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        User user;

        try
        {
            user = context.Users.FirstOrDefault(u => u.Username == Input.Username);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }
        catch
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        if (!PasswordHasher.VerifyPassword(user.Password, Input.Password))
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Type.ToString()),
        };
        
        var identity = new ClaimsIdentity(claims, "Cookie");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });


        return RedirectToPage("/Index");
    }
}