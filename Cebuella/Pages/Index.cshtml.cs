using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cebuella.Pages;

[AllowAnonymous]
public class Index : PageModel
{
    
    public string InstanceName { get; set; } = string.Empty;
    
    private readonly IConfiguration _configuration;

    public Index(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public void OnGet()
    {
        InstanceName = _configuration["InstanceName"] ?? string.Empty;
    }
}