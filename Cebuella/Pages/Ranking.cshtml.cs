using Cebuella.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

[AllowAnonymous]
public class Ranking : PageModel
{
    public List<User> Users { get; set; } = [];
    private readonly AppDbContext _context;
    public List<PointCategory> PointCategories { get; set; } = [];

    public Ranking(AppDbContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        Users = _context.Users.Where(u => u.Type == UserType.Student).ToList().OrderBy(u => -u.GetPoints()).ToList();
        PointCategories = _context.PointCategories.ToList();
    }
}