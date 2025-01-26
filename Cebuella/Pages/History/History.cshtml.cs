using System.Security.Claims;
using Cebuella.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Cebuella.Pages.History;

public class History : PageModel
{
    [BindProperty(SupportsGet = true)] 
    public string StudentUsername { get; set; }

    public string DailyReport { get; set; }

    private readonly AppDbContext context;

    public History(ILogger<IndexModel> logger, AppDbContext context)
    {
        this.context = context;
    }

    public IActionResult OnGet()
    {
        var user = HttpContext.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var username = user.FindFirst(ClaimTypes.Name)?.Value;

        if (role == "Student" && username != StudentUsername) return Redirect("/");
        
        if(!context.Users.Any(x => x.Username == StudentUsername)) return NotFound();

        var reports = context.Reports.Where(t => t.Username == StudentUsername).ToList();
        DailyReport = "";
        foreach (var report in reports)
        {
            DailyReport += "# " + report.Date.ToShortDateString() + "\n";
            DailyReport += report.Content + "\n\n";
        }
        
        return Page();
    }
    

}