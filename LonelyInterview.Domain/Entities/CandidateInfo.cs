using LonelyInterview.Domain.Interfaces;

namespace LonelyInterview.Domain.Entities;

public class CandidateInfo : IEntity
{
    public Guid Id { get; init; }

    public Candidate Candidate { get; init; }
    public string? Biography { get; init; }
    public string? WorkExperience { get; init; }

    private CandidateInfo() { }



}
