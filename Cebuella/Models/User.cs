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
    public int Weight { get; set; } 
    
    public long[] Points { get; set; } = [];

    private bool _computedSum = false;
    private long _totalPoints = 0;

    public long GetPoints()
    {
        if(!_computedSum) _totalPoints = Points.Sum();
        _computedSum = true;
        return _totalPoints;
    }
}

public enum UserType
{
    Student,
    Coach,
    Admin
}