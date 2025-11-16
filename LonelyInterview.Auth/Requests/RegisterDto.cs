using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Auth.Requests;

public record RegisterDto(string UserName, DateOnly? BirthDay, string? Specialty, string? Degree, int? GraduationYear,
    string Email, string? Telegram, string? WorkExperience, string Password);
