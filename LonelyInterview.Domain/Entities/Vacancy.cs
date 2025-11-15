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


    public HrManager ResponsibleHr { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? ClosedAt { get; set; }


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