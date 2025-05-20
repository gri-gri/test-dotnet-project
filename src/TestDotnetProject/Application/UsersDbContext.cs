using Microsoft.EntityFrameworkCore;
using TestDotnetProject.Domain;

namespace TestDotnetProject.Application;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions options) : base() { }
    
    public DbSet<User> Users { get; set; }
}
