



namespace LonelyInterview.Application.Requests;

public record RegisterHrDto(string UserName, DateOnly? BirthDay, string? Company,
                string Email, string? Telegram, string Password);
