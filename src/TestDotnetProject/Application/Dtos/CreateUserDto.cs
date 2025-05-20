namespace TestDotnetProject.Application.Dtos;

public record class CreateUserDto(
    string Login, string Password, string Name, int Gender, DateTime? Birthday, bool IsAdmin, string CreatorLogin);
