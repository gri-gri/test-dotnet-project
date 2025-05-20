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
        var user = new User(dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, dto.CreatorLogin);

        usersDbContext.Add(user);

        await usersDbContext.SaveChangesAsync();

        return user.Guid;
    }
}
