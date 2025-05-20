using Microsoft.EntityFrameworkCore;
using TestDotnetProject.Application.Dtos;
using TestDotnetProject.Application.Exceptions;
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
        await CheckLoginUnique(dto.Login);

        var user = new User(dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, dto.CreatorLogin);

        usersDbContext.Users.Add(user);

        await usersDbContext.SaveChangesAsync();

        return user.Guid;
    }

    public async Task ChangeInfoAsync(Guid guid, string name, int gender, DateTime? birthday, string modifierLogin)
    {
        var user = await FindAsync(guid);

        user.ChangeName(name, modifierLogin);
        user.ChangeGender(gender, modifierLogin);
        user.ChangeBirthday(birthday, modifierLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid guid, string password, string modifierLogin)
    {
        var user = await FindAsync(guid);

        user.ChangePassword(password, modifierLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task ChangeLoginAsync(Guid guid, string login, string modifierLogin)
    {
        await CheckLoginUnique(login);

        var user = await FindAsync(guid);

        user.ChangeLogin(login, modifierLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task<User> FindAsync(Guid guid)
    {
        return await usersDbContext.Users.FirstOrDefaultAsync(user => user.Guid == guid)
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
        var user = await FindByLoginAsync(login);

        usersDbContext.Users.Remove(user);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task RevokeAsync(string login, string revokerLogin)
    {
        var user = await FindByLoginAsync(login);

        user.Revoke(revokerLogin);

        await usersDbContext.SaveChangesAsync();
    }

    public async Task ReviveAsync(Guid guid, string reviverLogin)
    {
        var user = await FindAsync(guid);

        user.Revive(reviverLogin);

        await usersDbContext.SaveChangesAsync();
    }

    private async Task<User> FindByLoginAsync(string login)
    {
        return await usersDbContext.Users.FirstOrDefaultAsync(user => user.Login == login)
            ?? throw UserNotFoundException.FromAnyStringId(login);
    }

    private async Task CheckLoginUnique(string login)
    {
        var user = await GetByLoginAsync(login);

        if (user is not null)
        {
            throw LoginIsNotUniqueException.FromLogin(login);
        }
    }
}
