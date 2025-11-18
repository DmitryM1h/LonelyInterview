using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Application.Requests.Auth;

public record RegisterCandidateDto(string UserName, DateOnly? BirthDay, string? Specialty, string? Degree, int? GraduationYear,
    string Email, string? Telegram, string? WorkExperience, string Password);


public record RegisterCandidateResponse(string UserName, DateOnly? BirthDay, string? Specialty, string? Degree, int? GraduationYear,
    string Email, string? Telegram, string? WorkExperience, string Password);

