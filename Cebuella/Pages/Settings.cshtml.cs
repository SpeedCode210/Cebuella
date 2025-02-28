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
    

    
    private readonly AppDbContext _context;
    
    public Settings(AppDbContext context)
    {
        _context = context;
    }
    
    public IActionResult OnGet()
    {
        var user = HttpContext.User;
        var username = user.FindFirst(ClaimTypes.Role)?.Value;
        
        if(username == "Student") return Redirect("/");

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
        }
        _context.SaveChanges();
        return OnGet();
    }
    

}