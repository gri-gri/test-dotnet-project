using Microsoft.EntityFrameworkCore;

namespace TestDotnetProject;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions options) : base() { }
    
    public DbSet<User> Users { get; set; }
}
