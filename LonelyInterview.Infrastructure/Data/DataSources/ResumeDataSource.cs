using LonelyInterview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Infrastructure.Data.DataSources;

public class ResumeDataSource(LonelyInterviewContext _lonelyContext) : AbstractRepository<Resume, Guid>(_lonelyContext)
{

    public async Task<IEnumerable<Resume>> GetByCandidateIdWithVacancyInfoAsync(Guid candidateId, CancellationToken token = default)
    {
        return await _context.Resumes.Where(t => t.Candidate.Id == candidateId).Include(t => t.Vacancy).ToListAsync(token);
    }

}
