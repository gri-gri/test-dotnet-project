using Microsoft.EntityFrameworkCore;
using TestDotnetProject.Domain;

namespace TestDotnetProject.Application;

public class UsersRepository
{
    private readonly UsersDbContext usersDbContext;

    public UsersRepository(UsersDbContext usersDbContext)
    {
        this.usersDbContext = usersDbContext;
    }

    public async Task<Guid> CreateAsync(CreateUserDto dto)
    {
        var existingUserWithSameLogin = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Login == dto.Login);

        if (existingUserWithSameLogin is not null)
        {
            throw new LoginIsNotUniqueRepositoryException("Login is not unique", dto.Login);
        }

        var user = new User(dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, dto.CreatorLogin);

        usersDbContext.Users.Add(user);

        await usersDbContext.SaveChangesAsync();

        return user.Guid;
    }

    public async Task<List<User>> GetActiveAsync()
    {
        return await usersDbContext.Users
            .Where(user => user.RevokedOn == default)
            .OrderBy(user => user.CreatedOn)
            .ToListAsync();
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await usersDbContext.Users.FirstOrDefaultAsync(user => user.Login == login);
    }

    public async Task<User?> GetByLoginAndPasswordAsync(string login, string password)
    {
        return await usersDbContext.Users.FirstOrDefaultAsync(user => user.Login == login && user.Password == password);
    }

    public async Task<List<User>> GetOlderThanYearsAsync(int years)
    {
        var latestBirthdate = DateTime.UtcNow.AddYears(-years);

        return await usersDbContext.Users.Where(user => user.Birthday < latestBirthdate).ToListAsync();
    }
}
