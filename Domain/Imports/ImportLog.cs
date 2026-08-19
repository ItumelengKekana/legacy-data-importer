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
        _errors.Add(new ImportError(lineNumber, rawLine, reason));
    }

    public void Complete() => CompletedAt = DateTime.UtcNow;
}

public class ImportError(int lineNumber, string rawLine, string reason)
{
    public int Id { get; private set; }
    public int LineNumber { get; } = lineNumber;
    public string RawLine { get; } = rawLine.Length > 200 ? rawLine[..200] : rawLine;
    public string Reason { get; } = reason;
}