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

        return Page();
    }

    public IActionResult OnPost()
    {
        var user = HttpContext.User;
        var username = user.FindFirst(ClaimTypes.Role)?.Value;

        if (username == "Student") return Redirect("/");

        switch (Action)
        {
            case 0:
                string c = user.FindFirst(ClaimTypes.Name)?.Value;
                context.Tasks.Add(new()
                {
                    StudentId = StudentUsername,
                    Content = TaskContent,
                    Status = Status.NotAttempted,
                    CoachId = c
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

    class DiscordInfo
    {
        public string Token { get; set; }
        public ulong Guild { get; set; }
    }
}