using Cebuella.Models;
using Cebulla;
using Microsoft.EntityFrameworkCore;

namespace Cebuella;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<StudentTask> Tasks { get; set; }
    public DbSet<PointCategory> PointCategories { get; set; }

    public void EnsureAdminExists()
    {
        if (!Users.Any(x => x.Username == "admin"))
        {
            Users.Add(new()
            {
                FirstName = "Cebuella",
                LastName = "Admin",
                Email = "admin@example.com",
                Password = PasswordHasher.HashPassword("12345678"),
                Type = UserType.Admin,
                Username = "admin"
            });
            SaveChanges();
        }
    }
}