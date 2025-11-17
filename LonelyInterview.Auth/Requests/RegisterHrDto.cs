



namespace LonelyInterview.Auth.Requests;

public record RegisterHrDto(string UserName, DateOnly? BirthDay, 
                string Email, string? Telegram, string Password);
