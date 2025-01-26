using System.Security.Claims;
using Cebuella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty] public string DailyReport { get; set; }

    [BindProperty] public bool NewReport { get; set; }

    [BindProperty] public int Action { get; set; }
    
    public string Username { get; set; }

    public List<StudentTask> Tasks { get; set; } = new();
    
    [BindProperty]
    public Status TaskStatus { get; set; }
    
    [BindProperty]
    public int TaskId { get; set; }


    private readonly AppDbContext context;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public IActionResult OnGet()
    {
        
        var user = HttpContext.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var username = user.FindFirst(ClaimTypes.Name)?.Value;
        Username = username!;
        
        if(role != "Student") return Redirect("/Manage");


        var report = context.Reports.FirstOrDefault(t => t.Username == username && t.Date == DateTime.Now.Date);
        if (report == null)
        {
            NewReport = true;
        }
        else
        {
            NewReport = false;
            DailyReport = report.Content;
        }
        
        Tasks = context.Tasks.Where(t=>t.StudentId == username).ToList();
        Tasks.Sort((a, b) =>
        {
            if (a.Content.ToLower().Contains("important"))
            {
                return -1;
            }
            
            if (b.Content.ToLower().Contains("important"))
            {
                return 1;
            }

            return 0;
        });
        
        return Page();
    }

    public IActionResult OnPost()
    {
        switch (Action)
        {
            case 0:
                context.Reports.Add(new()
                {
                    Date = DateTime.Now.Date,
                    Username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value,
                    Content = DailyReport
                });
                context.SaveChanges();
                break;
            case 1:
                var user = HttpContext.User;
                var username = user.FindFirst(ClaimTypes.Name)?.Value;
                var report =
                    context.Reports.FirstOrDefault(t => t.Username == username && t.Date == DateTime.Now.Date);
                report!.Content = DailyReport;
                context.Reports.Update(report);
                context.SaveChanges();
                break;
            case 2:
                var task = context.Tasks.FirstOrDefault(t => t.Id == TaskId);
                task.Status = TaskStatus;
                context.Tasks.Update(task);
                context.SaveChanges();
                return Redirect("/");
                break;
        }

        return OnGet();
    }
}