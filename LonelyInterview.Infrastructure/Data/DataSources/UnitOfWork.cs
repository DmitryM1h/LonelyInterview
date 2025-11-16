using LonelyInterview.Domain.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Infrastructure.Data.DataSources;

public class UnitOfWork(LonelyInterviewContext _dbContext)
{
   // private IDbContextTransaction? _currentTransaction;

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    //public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    //{
    //    if (_currentTransaction != null)
    //    {
    //        throw new InvalidOperationException("A transaction is already in progress");
    //    }

    //    _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    //    return _currentTransaction;
    //}
    //public async ValueTask CommitAsync(CancellationToken cancellationToken = default)
    //{
    //    if (_currentTransaction is null) return;
    //    await _currentTransaction.CommitAsync(cancellationToken);
    //}

    //public async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
    //{
    //    if (_currentTransaction is null) return;

    //    await _currentTransaction.RollbackAsync(cancellationToken);
    //}

}
