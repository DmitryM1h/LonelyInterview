namespace LonelyInterview.Application.Requests.Auth;

public record RegisterHrDto(string UserName, DateOnly? BirthDay, string Company,
                string Email, string? Telegram, string Password, string Position, string PhoneNumber);


public record RegisterHrResponse(string UserName, DateOnly? BirthDay, string Company,
                string Email, string? Telegram, string Password, string Position, string PhoneNumber);
