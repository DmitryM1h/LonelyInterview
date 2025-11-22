using LonelyInterview.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace LonelyInterview.Infrastructure.Data.DataSources;

public class HrManagerDataSource(LonelyInterviewContext context) : AbstractRepository<HrManager, Guid>(context)
{

    public async Task<HrManager?> GetHrManagerByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.HrManagers.Include(t => t.Vacancies).Where(t => t.Id == id).AsTracking().FirstOrDefaultAsync();
    }


}
