using System.Security.Claims;
using Cebuella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

public class Settings : PageModel
{
    public List<PointCategory> PointCategories { get; set; }
    
    [BindProperty]
    public int Action { get; set; }
    
    [BindProperty]
    public string Category { get; set; }
    [BindProperty]
    public string NewCategory { get; set; }
    
    [BindProperty]
    public int ThemeId { get; set; }

    
    private readonly AppDbContext _context;
    
    public Settings(AppDbContext context)
    {
        _context = context;
    }

    public int GetTheme()
    {
        return HttpContext.Session.GetInt32("THEME") ?? 0;
    }
    
    public IActionResult OnGet()
    {
        var user = HttpContext.User;
        var username = user.FindFirst(ClaimTypes.Role)?.Value;
        
        PointCategories = _context.PointCategories.ToList();
        
        return Page();
    }

    public IActionResult OnPost()
    {
        PointCategory? pc;
        switch (Action)
        {
            case 0:
                _context.PointCategories.Add(new()
                {
                    Name = Category
                });
                break;
            case 1:
                pc = _context.PointCategories.FirstOrDefault(p => p.Name == Category);
                pc!.Name = NewCategory;
                _context.PointCategories.Update(pc);
                break;
            case 2:
                HttpContext.Session.SetInt32("THEME", ThemeId);
                break;
        }
        _context.SaveChanges();
        return OnGet();
    }
    

}