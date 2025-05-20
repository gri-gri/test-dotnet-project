using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestDotnetProject.Application;
using TestDotnetProject.Application.Dtos;
using TestDotnetProject.Application.Exceptions;
using TestDotnetProject.Domain;
using TestDotnetProject.Domain.Helpers;
using TestDotnetProject.Presentation.Dtos;

namespace TestDotnetProject.Presentation;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!user.Admin)
        {
            return Forbid();
        }

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
            dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, user.Login);

        try
        {
            var createdUserGuid = await usersRepository.CreateAsync(repositoryDto);

            return createdUserGuid;
        }
        catch (LoginIsNotUniqueException ex)
        {
            return BadRequest($"Login '{ex.Login}' is not unique");
        }
    }

    [HttpPut("{guid:guid}/info")]
    public async Task<ActionResult> ChangeInfoAsync([FromRoute] Guid guid, [FromBody] ChangeInfoRequestDto dto)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!(user.Admin || (user.Guid == guid && user.RevokedOn == default)))
        {
            return Forbid();
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

        try
        {
            await usersRepository.ChangeInfoAsync(guid, dto.Name, dto.Gender, dto.Birthday, user.Login);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with guid '{ex.Id}' was not found");
        }

        return Ok();
    }

    [HttpPut("{guid:guid}/password")]
    public async Task<ActionResult> ChangePasswordAsync([FromRoute] Guid guid, [FromBody] ChangePasswordRequestDto dto)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!(user.Admin || (user.Guid == guid && user.RevokedOn == default)))
        {
            return Forbid();
        }

        if (!dto.Password.IsAlphaNumeric())
        {
            return BadRequest($"Parameter {nameof(dto.Password)} with value '{dto.Password}' is not alphanumeric");
        }

        try
        {
            await usersRepository.ChangePasswordAsync(guid, dto.Password, user.Login);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with guid '{ex.Id}' was not found");
        }

        return Ok();
    }

    [HttpPut("{guid:guid}/login")]
    public async Task<ActionResult> ChangeLoginAsync([FromRoute] Guid guid, [FromBody] ChangeLoginRequestDto dto)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!(user.Admin || (user.Guid == guid && user.RevokedOn == default)))
        {
            return Forbid();
        }

        if (!dto.Login.IsAlphaNumeric())
        {
            return BadRequest($"Parameter {nameof(dto.Login)} with value '{dto.Login}' is not alphanumeric");
        }

        try
        {
            await usersRepository.ChangeLoginAsync(guid, dto.Login, user.Login);
        }
        catch (LoginIsNotUniqueException ex)
        {
            return BadRequest($"Login '{ex.Login}' is not unique");
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with guid '{ex.Id}' was not found");
        }

        return Ok();
    }

    [HttpGet("active")]
    // Here and after returning full user is bad, but anything other is not specified
    public async Task<ActionResult<List<User>>> GetActiveAsync()
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!user.Admin)
        {
            return Forbid();
        }

        return await usersRepository.GetActiveAsync();
    }

    [HttpGet("{login}")]
    public async Task<ActionResult<GetUserBriefResponseDto>> GetByLoginAsync([FromRoute] string login)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!user.Admin)
        {
            return Forbid();
        }

        var resultUser = await usersRepository.GetByLoginAsync(login);

        if (resultUser is null)
        {
            return NotFound();
        }

        return new GetUserBriefResponseDto(
            resultUser.Name, resultUser.Gender, resultUser.Birthday, resultUser.RevokedOn == default);
    }

    [HttpGet("{login}/password/{password}")] // Very bad decision, but the task requires
    public async Task<ActionResult<User>> GetByLoginAndPasswordAsync(
        [FromRoute] string login, [FromRoute] string password)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (user.Login != login)
        {
            return Forbid();
        }

        if (user.Password != password)
        {
            return NotFound();
        }

        return user;
    }

    [HttpGet("seniors")]
    public async Task<ActionResult<List<User>>> GetSeniorsAsync([FromQuery] int olderThanYears)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!user.Admin)
        {
            return Forbid();
        }

        if (olderThanYears < 1)
        {
            return BadRequest($"Parameter {nameof(olderThanYears)} must be positive integer, not '{olderThanYears}'");
        }

        return await usersRepository.GetOlderThanYearsAsync(olderThanYears);
    }

    [HttpDelete("{login}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] string login, [FromQuery] bool softDeletion)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!user.Admin)
        {
            return Forbid();
        }

        try
        {
            if (softDeletion)
            {
                await usersRepository.RevokeAsync(login, user.Login);
            }
            else
            {
                await usersRepository.DeleteAsync(login);
            }

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with login '{ex.Id}' was not found");
        }
    }

    [HttpPut("{guid:guid}/revoked-status")]
    public async Task<ActionResult> ReviveAsync([FromRoute] Guid guid, [FromBody] ReviveRequestDto dto)
    {
        var user = await usersRepository.FindAsync(
            Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Guid").Value));

        if (!user.Admin)
        {
            return Forbid();
        }

        if (dto.RevokedStatus)
        {
            return BadRequest(
                "RevokedStatus for this request must be false, because essentially this is the 'Revive' endpoint. " +
                "To revoke user call DELETE on user by login");
        }

        try
        {
            await usersRepository.ReviveAsync(guid, user.Login);

            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            return NotFound($"User with guid '{ex.Id}' was not found");
        }

    }
}
