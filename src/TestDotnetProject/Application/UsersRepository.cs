namespace TestDotnetProject.Application;

public class UsersRepository
{
    private readonly UsersDbContext usersDbContext;

    public UsersRepository(UsersDbContext usersDbContext)
    {
        this.usersDbContext = usersDbContext;
    }

    public async Task CreateAsync() {}
}
