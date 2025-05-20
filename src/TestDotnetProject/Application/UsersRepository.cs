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
        var existingUserWithSameLogin = await usersDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Login == dto.Login);

        if (existingUserWithSameLogin is not null)
        {
            throw LoginIsNotUniqueRepositoryException.FromLogin(dto.Login);
        }

        var user = new User(dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, dto.CreatorLogin);

        usersDbContext.Users.Add(user);

        await usersDbContext.SaveChangesAsync();

        return user.Guid;
    }

    public async Task ChangeInfoAsync(Guid guid, string name, int gender, DateTime? birthday, string modifierLogin)
    {
        var user = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Guid == guid)
            ?? throw UserNotFoundException.FromAnyStringId(guid.ToString());

        user.ChangeName(name, modifierLogin);
        user.ChangeGender(gender, modifierLogin);
        user.ChangeBirthday(birthday, modifierLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid guid, string password, string modifierLogin)
    {
        var user = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Guid == guid)
            ?? throw UserNotFoundException.FromAnyStringId(guid.ToString());

        user.ChangePassword(password, modifierLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task ChangeLoginAsync(Guid guid, string login, string modifierLogin)
    {
        var existingUserWithSameLogin = await usersDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Login == login);

        if (existingUserWithSameLogin is not null)
        {
            throw LoginIsNotUniqueRepositoryException.FromLogin(login);
        }

        var user = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Guid == guid)
            ?? throw UserNotFoundException.FromAnyStringId(guid.ToString());

        user.ChangeLogin(login, modifierLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task<User> FindUserAsync(Guid guid)
    {
        return await usersDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Guid == guid)
            ?? throw UserNotFoundException.FromAnyStringId(guid.ToString());
    }

    public async Task<List<User>> GetActiveAsync()
    {
        return await usersDbContext.Users
            .AsNoTracking()
            .Where(user => user.RevokedOn == default)
            .OrderBy(user => user.CreatedOn)
            .ToListAsync();
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await usersDbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Login == login);
    }

    public async Task<User?> GetByLoginAndPasswordAsync(string login, string password)
    {
        return await usersDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Login == login && user.Password == password);
    }

    public async Task<List<User>> GetOlderThanYearsAsync(int years)
    {
        var latestBirthdate = DateTime.UtcNow.AddYears(-years);

        return await usersDbContext.Users.AsNoTracking().Where(user => user.Birthday < latestBirthdate).ToListAsync();
    }

    public async Task DeleteAsync(string login)
    {
        var user = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Login == login)
            ?? throw UserNotFoundException.FromAnyStringId(login);

        usersDbContext.Users.Remove(user);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task RevokeAsync(string login, string revokerLogin)
    {
        var user = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Login == login)
            ?? throw UserNotFoundException.FromAnyStringId(login);

        user.Revoke(revokerLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task ReviveAsync(Guid guid, string reviverLogin)
    {
        var user = await usersDbContext.Users.FirstOrDefaultAsync(user => user.Guid == guid)
            ?? throw UserNotFoundException.FromAnyStringId(guid.ToString());

        user.Revive(reviverLogin);

        await usersDbContext.SaveChangesAsync();
    }
}
