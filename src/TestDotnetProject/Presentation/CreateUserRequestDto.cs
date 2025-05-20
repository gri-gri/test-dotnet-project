namespace TestDotnetProject.Presentation;

public record class CreateUserRequestDto(
    string Login, string Password, string Name, int Gender, DateTime? Birthday, bool IsAdmin);