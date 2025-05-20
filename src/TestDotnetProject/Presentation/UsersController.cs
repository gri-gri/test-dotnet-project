using Microsoft.AspNetCore.Mvc;
using TestDotnetProject.Application;
using TestDotnetProject.Domain;

namespace TestDotnetProject.Presentation;

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
    public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequestDto dto)
    {
        if (!dto.Login.IsAlphaNumeric())
        {
            return BadRequest($"Parameter {nameof(dto.Login)} with value '{dto.Login}' is not alphanumeric");
        }

        if (!dto.Password.IsAlphaNumeric())
        {
            return BadRequest($"Parameter {nameof(dto.Password)} with value '{dto.Password}' is not alphanumeric");
        }

        if (!dto.Name.ConsistsFromLatinRussianLetters())
        {
            return BadRequest(
                $"Parameter {nameof(dto.Name)} with value '{dto.Name}' " +
                "does not consist from latin and/or russian letters");
        }

        if (dto.Gender < 0 || dto.Gender > 2)
        {
            return BadRequest($"Parameter {nameof(dto.Gender)} must be 0, 1 or 2, not '{dto.Gender}'");
        }

        var repositoryDto = new CreateUserDto(
            dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, "defaultAdmin");

        try
        {
            var userGuid = await usersRepository.CreateAsync(repositoryDto);

            return userGuid;
        }
        catch (LoginIsNotUniqueRepositoryException ex)
        {
            return BadRequest($"Login '{ex.Login}' is not unique");
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<User>>> GetActiveUsers()
    {
        return await usersRepository.GetActiveUsers();
    }
}
