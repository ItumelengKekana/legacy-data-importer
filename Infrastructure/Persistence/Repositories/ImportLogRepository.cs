using Domain.Imports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ImportLogRepository(AppDbContext dbContext) : IImportLogRepository
{
    public async Task AddAsync(ImportLog importLog, CancellationToken cancellationToken = default)
    {
        await dbContext.ImportLogs.AddAsync(importLog, cancellationToken);
    }

    public Task<ImportLog?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return dbContext.ImportLogs
            .Include(l => l.Errors)
            .FirstOrDefaultAsync(l => l.BatchId == batchId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}