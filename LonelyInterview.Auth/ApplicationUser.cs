

using Microsoft.AspNetCore.Identity;

namespace LonelyInterview.Auth;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? Telegram { get; private set; }
    public DateOnly? BirthDay { get; private set; }

    private ApplicationUser() { }




    public static ApplicationUser CreateFromRegisterDto(string UserName, DateOnly? BirthDay,
                    string Email, string? Telegram, string Password)
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            throw new ArgumentNullException("Missing necessary fields");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = UserName.Trim(),
            Email = Email.Trim().ToLower(),     
            
        };

        if (!string.IsNullOrWhiteSpace(Telegram))
            user.SetTelegram(Telegram.Trim());

        if (BirthDay.HasValue)
            user.SetBirthDay(BirthDay.Value);

        return user;
    }

    public void SetTelegram(string telegram)
    {
        if (string.IsNullOrWhiteSpace(telegram))
            throw new ArgumentException("Telegram cannot be empty", nameof(telegram));

        if (telegram.Length < 5)
            throw new ArgumentException("Telegram must be at least 5 characters", nameof(telegram));

        Telegram = telegram;
    }

    public void SetBirthDay(DateOnly birthDay)
    {
        if (birthDay > DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("Birthday cannot be in the future", nameof(birthDay));

        if (birthDay < DateOnly.FromDateTime(DateTime.Now.AddYears(-150)))
            throw new ArgumentException("Invalid birthday", nameof(birthDay));

        BirthDay = birthDay;
    }

}
