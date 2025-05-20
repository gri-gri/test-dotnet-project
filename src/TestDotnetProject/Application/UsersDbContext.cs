using Microsoft.EntityFrameworkCore;
using TestDotnetProject.Domain;

namespace TestDotnetProject.Application;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(user => user.Guid);
    }
}
