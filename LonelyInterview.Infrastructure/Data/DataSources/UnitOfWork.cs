using LonelyInterview.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Infrastructure.Data.DataSources;

public class UnitOfWork(LonelyInterviewContext _dbContext) : IUnitOfWork
{
    
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
       await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
