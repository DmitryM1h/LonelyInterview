using LonelyInterview.Domain.Interfaces;
using System;


namespace LonelyInterview.Domain.Entities;

public class Candidate : IEntity
{
    public Guid Id { get; init; }


    private List<Resume> _resumes = new List<Resume>();
    public IReadOnlyCollection<Resume>? Resumes => _resumes.AsReadOnly();


    public CandidateInfo? Info { get; private set; }

    public void AddResume(Resume resume)
    {
        // TODO Валидация
        _resumes.Add(resume);
    }

    public void AddInfo(CandidateInfo info)
    {
        // TODO валидация
        Info = info;
    }

    private Candidate() { }

}
