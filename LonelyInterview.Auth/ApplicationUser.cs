

using LonelyInterview.Auth.Requests;
using Microsoft.AspNetCore.Identity;

namespace LonelyInterview.Auth;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Telegram { get; private set; } = null!;
    public DateOnly BirthDay { get; private set; }

    private ApplicationUser() { }




    public static ApplicationUser CreateFromRegisterDto(RegisterDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName.Trim(),
            Email = dto.Email.Trim().ToLower(),     
            
        };

        if (!string.IsNullOrWhiteSpace(dto.Telegram))
            user.SetTelegram(dto.Telegram.Trim());

        if (dto.BirthDay.HasValue)
            user.SetBirthDay(dto.BirthDay.Value);

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
