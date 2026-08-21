namespace Domain.Imports;

public class ImportLog
{
    public int Id { get; }
    public Guid BatchId { get; } = Guid.NewGuid();
    public DateTime StartedAt { get; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; private set; }
    public int TotalProcessed { get; private set; }
    public int CreatedCount { get; private set; }
    public int UpdatedCount { get; private set; }
    public int FailedCount { get; private set; }

    private readonly List<ImportError> _errors = [];
    public IReadOnlyCollection<ImportError> Errors => _errors.AsReadOnly();

    public void RecordSuccess(bool isNew)
    {
        TotalProcessed++;
        if (isNew) CreatedCount++;
        else UpdatedCount++;
    }

    public void RecordFailure(int lineNumber, string rawLine, string reason)
    {
        TotalProcessed++;
        FailedCount++;
        _errors.Add(new ImportError(0, lineNumber, rawLine, reason));
    }

    public void Complete() => CompletedAt = DateTime.UtcNow;
}

public record ImportError(int Id, int lineNumber, string rawLine, string reason);