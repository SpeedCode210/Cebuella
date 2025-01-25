using System.Security.Claims;
using Cebuella.Models;
using Cebulla;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

public class Profile : PageModel
{
    public required User User { get; set; }
    [BindProperty] public string ErrorMessage { get; set; }
    [BindProperty] public string FirstName { get; set; }
    [BindProperty] public string LastName { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string NewPassword { get; set; }
    [BindProperty] public string NewPasswordConfirmation { get; set; }

    private readonly AppDbContext context;

    public Profile(AppDbContext context)
    {
        this.context = context;
    }

    public void OnGet()
    {
        var user = HttpContext.User;

        // Retrieve the username (or other claims) from the principal
        var username = user.FindFirst(ClaimTypes.Name)?.Value;

        User = context.Users.FirstOrDefault(t => t.Username == username);

        FirstName = User.FirstName;
        LastName = User.LastName;
        Email = User.Email;
    }

    public void OnPost()
    {
        var user = HttpContext.User;

        // Retrieve the username (or other claims) from the principal
        var username = user.FindFirst(ClaimTypes.Name)?.Value;

        var realUser = context.Users.FirstOrDefault(t => t.Username == username);
        realUser.Email = Email;
        realUser.LastName = LastName;
        realUser.FirstName = FirstName;

        if (!String.IsNullOrEmpty(NewPassword) && NewPassword == NewPasswordConfirmation)
        {
            realUser.Password = PasswordHasher.HashPassword(NewPassword);
        }
        else if (!String.IsNullOrEmpty(NewPassword) || !String.IsNullOrEmpty(NewPasswordConfirmation))
        {
            ErrorMessage = "Passwords do not match!";
        }

        context.Update(realUser);
        User = realUser;
        context.SaveChanges();

        FirstName = User.FirstName;
        LastName = User.LastName;
        Email = User.Email;
    }
}