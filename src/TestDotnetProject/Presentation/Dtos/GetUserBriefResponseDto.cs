namespace TestDotnetProject.Presentation.Dtos;

public record class GetUserBriefResponseDto(string Name, int Gender, DateTime? Birthday, bool IsActive);
