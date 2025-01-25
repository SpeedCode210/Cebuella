using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

public class Logout : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        // Sign out the user
        await HttpContext.SignOutAsync();
        return RedirectToPage("/Login");
    }
}