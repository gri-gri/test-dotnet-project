using Microsoft.AspNetCore.Mvc;

namespace TestDotnetProject;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersRepository usersRepository;

    public UsersController(UsersRepository usersRepository) : base()
    {
        this.usersRepository = usersRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        await usersRepository.CreateAsync();

        return Created();
    }
}
