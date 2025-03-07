using System.Security.Claims;
using Cebuella.Models;
using Cebulla;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Cebuella.Pages;

public class Manage : PageModel
{
    public List<User> Users { get; set; }
    public List<int> UnsolvedTasks { get; set; } 
    public List<bool> SubmittedReport { get; set; } 
    
    [BindProperty]
    public int Action { get; set; }
    
    [BindProperty]
    public string Username { get; set; }
    
    [BindProperty]
    public string? DiscordId { get; set; }
    
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
        Users.Sort((a, b) =>
        {
            if(a.Type - b.Type != 0) return a.Type - b.Type;
            return a.Weight - b.Weight;
        });
        int[] unsolvedtasks = new int[Users.Count];
        bool[] submittedReport = new bool[Users.Count];
        for (int i = 0; i < Users.Count; i++)
        {
            unsolvedtasks[i] =
                context.Tasks.Count(t => t.StudentId == Users[i].Username && t.Status != Status.Completed);
            submittedReport[i] = context.Reports.Any(t=> t.Username == Users[i].Username && t.Date == DateTime.Now.Date);
        }
        UnsolvedTasks = unsolvedtasks.ToList();
        SubmittedReport = submittedReport.ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        User? usr;
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
                usr = context.Users.FirstOrDefault(u => u.Username == Username);
                usr!.Type = UserType.Coach;
                context.Users.Update(usr!);
                break;
            case 3: 
                usr = context.Users.FirstOrDefault(u => u.Username == Username);
                usr!.DiscordChannel = DiscordId ?? "";
                context.Users.Update(usr!);
                context.SaveChanges();
                return RedirectToPage("/Manage");
                break;
            case 4:
                List<string> peopleIds = new();
                var users = context.Users.Where(s=>s.Type == UserType.Student).ToList();
                foreach (var user in users)
                {
                    if(!context.Reports.Any(r => r.Username == user.Username && r.Date == DateTime.Now.Date) && user.DiscordChannel != "") peopleIds.Add(user.DiscordChannel);
                }
                RemindPeople(peopleIds);
                break;
            case 5:
                usr = context.Users.FirstOrDefault(u => u.Username == Username);
                usr!.Type = UserType.Student;
                context.Users.Update(usr!);
                break;
            case 6:
                usr = context.Users.FirstOrDefault(u => u.Username == Username);
                usr!.Weight -= 1;
                context.Users.Update(usr!);
                break;
            case 7:
                usr = context.Users.FirstOrDefault(u => u.Username == Username);
                usr!.Weight += 1;
                context.Users.Update(usr!);
                break;
        }
        context.SaveChanges();
        return OnGet();
    }
    
    async void RemindPeople(List<string> people)
    {
        var _client = new DiscordSocketClient();
        var config = JsonConvert.DeserializeObject<DiscordInfo>(System.IO.File.ReadAllText("discord.json"));

        await _client.LoginAsync(TokenType.Bot, config.Token);
        await _client.StartAsync();
        _client.Ready += async () =>
        {
            foreach(var c in people)
            {
                var channel = c;
                var embed = new EmbedBuilder()
                {
                    Color = Color.Teal,
                    Title = "Daily Report Reminder",
                    Description = "Please send today's report on the website !",
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
            }

            await _client.LogoutAsync();
            _client.Dispose();
        };
    }

}