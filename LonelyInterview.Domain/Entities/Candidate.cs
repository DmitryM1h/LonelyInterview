using LonelyInterview.Domain.Interfaces;


namespace LonelyInterview.Domain.Entities;


public class Candidate : IEntity
{
    public Guid Id { get; private set; }
    //TODO Денормализация XD

    private List<Resume> _resumes = new List<Resume>();
    public IReadOnlyCollection<Resume>? Resumes => _resumes.AsReadOnly();


    public CandidateInfo Info { get; private set; }

    public void AddResume(Resume resume)
    {
        _resumes.Add(resume);
    }

    public void UpdateInfo(CandidateInfo info)
    {

        if (Info is null)
        {
            var candInfo = CandidateInfo.Create(/*info.Id,*/ info.Specialty, info.Degree, info.GraduationYear, info.WorkExperience);
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
            candidate.Info = info;
        return candidate;
    }

}
