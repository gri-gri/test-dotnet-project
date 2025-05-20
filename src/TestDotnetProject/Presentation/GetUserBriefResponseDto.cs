namespace TestDotnetProject.Presentation;

public record class GetUserBriefResponseDto(string Name, int Gender, DateTime? Birthday, bool IsActive);
