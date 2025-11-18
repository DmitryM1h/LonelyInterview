using LonelyInterview.Domain.Interfaces;


namespace LonelyInterview.Domain.Entities;

public class Resume : IEntity
{
    public Guid Id { get; init; }

    public Candidate Candidate { get; init; }

    public Vacancy Vacancy { get; init; }

    public ResumeStatus Status { get; set; } = ResumeStatus.Submitted;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;


    public string? GitHubUrl { get; init; }
    public string? ActualSkills { get; init; }
    public string? PassiveSkills { get; init; }

    private Resume() { }
    
}

public enum ResumeStatus
{
    Submitted,    
    UnderReview,
    Rejected,
    Accepted
}