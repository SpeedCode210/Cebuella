using System.Security.Claims;
using Cebuella.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Cebuella.Pages.Student;

public class Student : PageModel
{
    [BindProperty(SupportsGet = true)] // Allows binding from the URL
    public string StudentUsername { get; set; }

    public string DailyReport { get; set; }

    public List<StudentTask> Tasks { get; set; } = new();

    [BindProperty] public string TaskContent { get; set; }

    [BindProperty] public int Action { get; set; }

    [BindProperty] public int TaskId { get; set; }

    public List<PointCategory> PointCategories { get; set; } = [];

    [BindProperty]
    public List<long> Points { get; set; } = [];
    
    [BindProperty]
    public List<long> DeltaPoints { get; set; } = [];

    private readonly AppDbContext context;

    public Student(ILogger<IndexModel> logger, AppDbContext context)
    {
        this.context = context;
    }

    public IActionResult OnGet()
    {
        var user = HttpContext.User;
        var username = user.FindFirst(ClaimTypes.Role)?.Value;

        if (username == "Student") return Redirect("/");

        if (!context.Users.Any(x => x.Username == StudentUsername)) return NotFound();

        var report = context.Reports.FirstOrDefault(t => t.Username == StudentUsername && t.Date == DateTime.Now.Date);
        if (report == null)
        {
            DailyReport = "No Report yet.";
        }
        else
        {
            DailyReport = report.Content;
        }

        Tasks = context.Tasks.Where(t => t.StudentId == StudentUsername).ToList();

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

        PointCategories = context.PointCategories.ToList();

        Points = context.Users.First(u => u.Username == StudentUsername).Points.ToList();

        while (Points.Count < PointCategories.Count) Points.Add(0);
        
        DeltaPoints = new List<long>();
        for(int i = 0; i < PointCategories.Count; i++) DeltaPoints.Add(0);

        return Page();
    }

    public IActionResult OnPost()
    {
        var username = HttpContext.User;
        var role = username.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Student") return Redirect("/");
        
        var usr = context.Users.FirstOrDefault(u => u.Username == StudentUsername)!;
        
        switch (Action)
        {
            case 0:
                string? c = username.FindFirst(ClaimTypes.Name)?.Value;
                context.Tasks.Add(new()
                {
                    StudentId = StudentUsername,
                    Content = TaskContent,
                    Status = Status.NotAttempted,
                    CoachId = c!
                });
                context.SaveChanges();
                var h = context.Users.FirstOrDefault(u => u.Username == StudentUsername)!.DiscordChannel;
                if (h != "")
                {
                    InformStudent(h, TaskContent, c);
                }

                TaskContent = "";
                break;
            case 1:
                context.Tasks.Remove(new() { Id = TaskId });
                context.SaveChanges();
                break;
            case 2:
                usr.Points = Points.ToArray();
                context.Users.Update(usr);
                context.SaveChanges();
                break;
            case 3:
                Points = usr.Points.ToList();
                while (Points.Count < DeltaPoints.Count) Points.Add(0);
                usr.Points = Points.ToArray();
                for(int i = 0; i < Points.Count; i++) usr.Points[i] += DeltaPoints[i];
                context.Users.Update(usr);
                context.SaveChanges();
                break;
        }

        return OnGet();
    }


    async void InformStudent(string channel, string content, string coach)
    {
        var _client = new DiscordSocketClient();
        var config = JsonConvert.DeserializeObject<DiscordInfo>(System.IO.File.ReadAllText("discord.json"));

        await _client.LoginAsync(TokenType.Bot, config.Token);
        await _client.StartAsync();
        _client.Ready += async () =>
        {
            var embed = new EmbedBuilder()
            {
                Color = Color.Teal,
                Title = "New Task from Coach " + coach,
                Description = content,
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Cebuella - Student Progress Tracker"
                },
                ThumbnailUrl =
                    "https://raw.githubusercontent.com/SpeedCode210/Cebuella/refs/heads/master/Cebuella/wwwroot/cebuella.png"
            }.Build();
            if (channel.Contains("x"))
            {
                var a = channel.Split('x');
                config.Guild = ulong.Parse(a[0]);
                channel = a[1];
            }

            await _client.GetGuild(config.Guild).GetTextChannel(ulong.Parse(channel)).SendMessageAsync(embed: embed);

            await _client.LogoutAsync();
            _client.Dispose();
        };
    }
}