using LonelyInterview.Domain.Interfaces;
using System.Security.Cryptography;


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

    public static Resume Create(/*Candidate candidate,*/ Vacancy vacancy, string? gitHubUrl = null, string? actualSkills = null, string? passiveSkills = null)
    {
        return new Resume
        {
            //Id = Guid.NewGuid(),
            //Candidate = candidate,
            Vacancy = vacancy,
            GitHubUrl = gitHubUrl,
            ActualSkills = actualSkills,
            PassiveSkills = passiveSkills,
            Status = ResumeStatus.Submitted,
            CreatedAt = DateTime.UtcNow
        };
    }

}

public enum ResumeStatus
{
    Submitted,    
    UnderReview,
    Rejected,
    Accepted
}