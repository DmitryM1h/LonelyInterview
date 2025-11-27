using LonelyInterview.Domain.Models;
using LonelyInterview.Domain.Repository;
using Microsoft.EntityFrameworkCore;


namespace LonelyInterview.Infrastructure.Data.DataSources;

public class CandidateDataSource(LonelyInterviewContext _context) : AbstractRepository<Candidate, Guid>(_context)
{

    public async Task<Candidate?> GetCandidateWithInfo(Guid candidateId, CancellationToken token = default)
    {
        return await _context.Candidates.Include(t => t.Info).Where(t => t.Id == candidateId).FirstOrDefaultAsync();
    }

    public async Task<List<Resume>> GetAppliedResumesWithVacancyAsync(Guid candidateId, CancellationToken token = default)
    {
        return await _context.Resumes.Include(t => t.Vacancy).Where(t => t.Candidate.Id == candidateId).ToListAsync();
    }

}
