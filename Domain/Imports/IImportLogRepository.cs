namespace Domain.Imports;

public interface IImportLogRepository
{
    Task AddAsync(ImportLog importLog, CancellationToken cancellationToken = default);
    Task<ImportLog?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
