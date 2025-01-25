namespace Cebuella;

public interface IUserService
{
    Task<string> GetUserFullNameAsync(string userId);
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetUserFullNameAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            return user.FirstName + " " + user.LastName;
        }

        return "";
    }
}