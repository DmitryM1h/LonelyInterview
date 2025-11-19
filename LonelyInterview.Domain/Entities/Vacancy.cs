using LonelyInterview.Domain.Interfaces;


namespace LonelyInterview.Domain.Entities;

public class Vacancy : IEntity
{
    public Guid Id { get; init; }

    private List<Resume> _resumes = new();

    public IReadOnlyCollection<Resume> Resumes => _resumes.AsReadOnly();
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ShortDescription { get; set; }

    public string RequiredSkills { get; set; } = null!;
    public string? NiceToHaveSkills { get; set; }
    public int? MinYearsOfExperience { get; set; }

    public string Location { get; set; } = null!;
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public string? Currency { get; set; } = "RUB";


    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public VacancyStatus Status { get; set; } = VacancyStatus.Draft;
    public WorkFormat WorkFormat { get; set; } = WorkFormat.Office;


    public bool isActive => Status == VacancyStatus.Published;

    public HrManager ResponsibleHr { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? ClosedAt { get; set; }


    public void SetStatus(VacancyStatus status)
    {
        Status = status;
    }

    public static Vacancy Create(
        string title,
        string description,
        string requiredSkills,
        string location,
        HrManager responsibleHr,
        string? shortDescription = null,
        string? niceToHaveSkills = null,
        int? minYearsOfExperience = null,
        decimal? salaryFrom = null,
        decimal? salaryTo = null,
        string? currency = "RUB",
        EmploymentType employmentType = EmploymentType.FullTime,
        WorkFormat workFormat = WorkFormat.Office)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (string.IsNullOrWhiteSpace(requiredSkills))
            throw new ArgumentException("Required skills cannot be null or empty", nameof(requiredSkills));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty", nameof(location));

        if (responsibleHr == null)
            throw new ArgumentNullException(nameof(responsibleHr));

        if (salaryFrom.HasValue && salaryTo.HasValue && salaryFrom > salaryTo)
            throw new ArgumentException("SalaryFrom cannot be greater than SalaryTo");

        if (minYearsOfExperience.HasValue && minYearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(minYearsOfExperience));

        if (!string.IsNullOrWhiteSpace(currency) && currency.Length != 3)
            throw new ArgumentException("Currency must be 3-letter code", nameof(currency));

        var vacancy = new Vacancy
        {
            Title = title.Trim(),
            Description = description.Trim(),
            RequiredSkills = requiredSkills.Trim(),
            Location = location.Trim(),
          //  ResponsibleHr = responsibleHr,
            ShortDescription = string.IsNullOrWhiteSpace(shortDescription) ? null : shortDescription.Trim(),
            NiceToHaveSkills = string.IsNullOrWhiteSpace(niceToHaveSkills) ? null : niceToHaveSkills.Trim(),
            MinYearsOfExperience = minYearsOfExperience,
            SalaryFrom = salaryFrom,
            SalaryTo = salaryTo,
            Currency = string.IsNullOrWhiteSpace(currency) ? "RUB" : currency.Trim().ToUpper(),
            EmploymentType = employmentType,
            WorkFormat = workFormat,
            Status = VacancyStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return vacancy;
    }

    private Vacancy() { }
}

public enum VacancyStatus
{
    Draft,           
    Published,      
    OnHold,        
    Closed,        
    Archived
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Freelance,
    Internship,
    Remote
}

public enum WorkFormat
{
    Office,
    Remote,
    Hybrid,
}