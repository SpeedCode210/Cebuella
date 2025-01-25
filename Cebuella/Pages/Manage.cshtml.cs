using System.Security.Claims;
using Cebuella.Models;
using Cebulla;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

public class Manage : PageModel
{
    public List<User> Users { get; set; }
    
    [BindProperty]
    public int Action { get; set; }
    
    [BindProperty]
    public string Username { get; set; }
    
    [BindProperty]
    public string DiscordId { get; set; }
    
    private readonly AppDbContext context;

    public Manage(AppDbContext context)
    {
        this.context = context;
    }
    
    public IActionResult OnGet()
    {
        var user = HttpContext.User;
        var username = user.FindFirst(ClaimTypes.Role)?.Value;
        
        if(username == "Student") return Redirect("/");

        Users = context.Users.ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        switch (Action)
        {
            case 0:
                context.Users.Add(new()
                {
                    Username = Username,
                    Email = "",
                    Password = PasswordHasher.HashPassword(Username + "_2025"),
                    Type = UserType.Student,
                    FirstName = Username,
                    LastName = ""
                });
                break;
            case 1:
                context.Users.Remove(new User() { Username = Username });
                break;
            case 2:
                var usr = context.Users.FirstOrDefault(u => u.Username == Username);
                usr!.Type = UserType.Coach;
                context.Users.Update(usr!);
                break;
            case 3:
                var usr2 = context.Users.FirstOrDefault(u => u.Username == Username);
                usr2!.DiscordChannel = DiscordId;
                context.Users.Update(usr2!);
                context.SaveChanges();
                return RedirectToPage("/Manage");
                break;
        }
        context.SaveChanges();
        return OnGet();
    }
}