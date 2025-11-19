using LonelyInterview.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Infrastructure.Data.DataSources
{
    public class VacancyDataSource(LonelyInterviewContext _context) : AbstractRepository<Vacancy, Guid>(_context)
    {
        public async Task<IEnumerable<Vacancy>> GetVacanciesByHrIdAsync(Guid hrId, CancellationToken token = default)
        {
            return await _context.Vacancies.Where(t => t.ResponsibleHr.Id == hrId).ToListAsync(token);
        }
    }
}
