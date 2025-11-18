using LonelyInterview.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Infrastructure.Data.DataSources
{
    public class VacancyDataSource(LonelyInterviewContext _context) : AbstractRepository<Vacancy, Guid>(_context)
    {

    }
}
