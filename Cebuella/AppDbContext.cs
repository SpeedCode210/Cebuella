using Cebuella.Models;
using Cebulla;
using Microsoft.EntityFrameworkCore;

namespace Cebuella;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
        if (!Users.Any())
        {
            Users.Add(new()
            {
                FirstName = "Raouf",
                LastName = "Ould Ali",
                Email = "raouf.ouldali@algerianoi.com",
                Password = PasswordHasher.HashPassword("12345678"),
                Type = UserType.Coach,
                Username = "speedcode"
            });
            SaveChanges();
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<StudentTask> Tasks { get; set; }
}