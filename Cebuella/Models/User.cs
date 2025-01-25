using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Cebuella.Models;

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public string DiscordChannel { get; set; } = "";
    public string Password { get; set; }
    [Key]
    public string Username { get; set; }
    public UserType Type { get; set; }
}

public enum UserType
{
    Student,
    Coach
}