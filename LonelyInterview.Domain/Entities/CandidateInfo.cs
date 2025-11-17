using LonelyInterview.Domain.Interfaces;

namespace LonelyInterview.Domain.Entities;

public class CandidateInfo : IEntity
{
    public Guid Id { get; init; }

    public Candidate Candidate { get; private set; }
    public string? Specialty { get; private set; }
    public string? Degree { get; private set; }
    public int ? GraduationYear { get; private set; }
    public string? WorkExperience { get; private set; }

    private CandidateInfo() { }


    public static CandidateInfo Create(Guid id, string? specialty, string? degree, int? graduationYear, string? workExperience)
    {
        int? validatedGraduationYear = null;

        if (graduationYear >= 1900 && graduationYear <= DateTime.Now.Year + 5)
        {
            validatedGraduationYear = graduationYear;
        }
        else
        {
            throw new ArgumentException($"Graduation year must be between 1900 and {DateTime.Now.Year + 5}");
        }


        return new CandidateInfo
        {
            Id = id,
            Specialty = string.IsNullOrWhiteSpace(specialty) ? null : specialty.Trim(),
            Degree = string.IsNullOrWhiteSpace(degree) ? null : degree.Trim(),
            GraduationYear = validatedGraduationYear,
            WorkExperience = string.IsNullOrWhiteSpace(workExperience) ? null : workExperience.Trim()
        };
    }

    public static CandidateInfo Create(string? specialty, string? degree, int? graduationYear, string? workExperience)
    {
        int? validatedGraduationYear = null;

        if (graduationYear >= 1900 && graduationYear <= DateTime.Now.Year + 5)
        {
            validatedGraduationYear = graduationYear;
        }
        else
        {
            throw new ArgumentException($"Graduation year must be between 1900 and {DateTime.Now.Year + 5}");
        }


        return new CandidateInfo
        {
            Specialty = string.IsNullOrWhiteSpace(specialty) ? null : specialty.Trim(),
            Degree = string.IsNullOrWhiteSpace(degree) ? null : degree.Trim(),
            GraduationYear = validatedGraduationYear,
            WorkExperience = string.IsNullOrWhiteSpace(workExperience) ? null : workExperience.Trim()
        };
    }

    public void Update(string? specialty, string? degree, int? graduationYear, string? workExperience)
    {
        int? validatedGraduationYear = null;
        if (graduationYear.HasValue)
        {
            if (graduationYear >= 1900 && graduationYear <= DateTime.Now.Year + 5)
            {
                validatedGraduationYear = graduationYear;
            }
            else
            {
                throw new ArgumentException($"Graduation year must be between 1900 and {DateTime.Now.Year + 5}");
            }
        }

        Specialty = string.IsNullOrWhiteSpace(specialty) ? null : specialty.Trim();
        Degree = string.IsNullOrWhiteSpace(degree) ? null : degree.Trim();
        GraduationYear = validatedGraduationYear;
        WorkExperience = string.IsNullOrWhiteSpace(workExperience) ? null : workExperience.Trim();
    }
}
