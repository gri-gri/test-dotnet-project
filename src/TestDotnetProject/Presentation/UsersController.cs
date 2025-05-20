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
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CreateUserRequestDto dto)
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
    // Here and after returning full user is bad, but anything other is not specified
    public async Task<ActionResult<List<User>>> GetActiveAsync()
    {
        return await usersRepository.GetActiveAsync();
    }

    [HttpGet("{login}")]
    public async Task<ActionResult<GetUserBriefResponseDto>> GetByLoginAsync([FromRoute] string login)
    {
        var user = await usersRepository.GetByLoginAsync(login);

        if (user is null)
        {
            return NotFound();
        }

        return new GetUserBriefResponseDto(user.Name, user.Gender, user.Birthday, user.RevokedOn == default);
    }

    [HttpGet("{login}/password/{password}")] // Very bad decision, but the task is so
    public async Task<ActionResult<User>> GetByLoginAndPasswordAsync(
        [FromRoute] string login, [FromRoute] string password)
    {
        var user = await usersRepository.GetByLoginAndPasswordAsync(login, password);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpGet("seniors")]
    public async Task<ActionResult<List<User>>> GetSeniorsAsync([FromQuery] int olderThanYears)
    {
        if (olderThanYears < 1)
        {
            return BadRequest($"Parameter {nameof(olderThanYears)} must be positive integer, not '{olderThanYears}'");
        }

        return await usersRepository.GetOlderThanYearsAsync(olderThanYears);
    }

    [HttpDelete("{login}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] string login, [FromQuery] bool softDeletion)
    {
        try
        {
            if (softDeletion)
            {
                await usersRepository.RevokeAsync(login, "defaultAdmin");
            }
            else
            {
                await usersRepository.DeleteAsync(login);
            }

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with login '{ex.Login}' was not found");
        }
    }

    [HttpPatch("{login}")]
    public async Task<ActionResult> ReviveAsync([FromRoute] string login, [FromBody] ReviveRequestDto dto)
    {
        if (dto.RevokedOn is not null || dto.RevokedBy is not null)
        {
            return BadRequest(
                "Both RevokedOn and RevokedBy for this request must be null, " +
                "because essentially this is the 'Revive' endpoint");
        }

        try
        {
            await usersRepository.ReviveAsync(login);

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with login '{ex.Login}' was not found");
        }

    }
}
