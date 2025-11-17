using LonelyInterview.Domain.Interfaces;
using System;
using System.ComponentModel;


namespace LonelyInterview.Domain.Entities;


public class Candidate : IEntity
{
    public Guid Id { get; private set; }


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
        if (Info is null)
        {
            Info = info;
        }

    }

    public void UpdateInfo(CandidateInfo info)
    {

        if (Info is null)
        {
            var candInfo = CandidateInfo.Create(info.Id, info.Specialty, info.Degree, info.GraduationYear, info.WorkExperience);
            Info = candInfo;
            return;
        }

        Info.Update(info.Specialty, info.Degree, info.GraduationYear, info.WorkExperience);

    }

    private Candidate() { }

    public static Candidate CreateCandidate(Guid id, CandidateInfo? info)
    {
        var candidate = new Candidate() { Id = id};
        if (info is not null)
            candidate.AddInfo(info);
        return candidate;
    }

}
