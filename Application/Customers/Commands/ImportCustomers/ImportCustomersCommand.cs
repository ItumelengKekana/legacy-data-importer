using Application.Common.Interfaces;
using Domain.Customers;
using Domain.Imports;

namespace Application.Customers.Commands.ImportCustomers;

public record ImportCustomersCommand(Stream FileStream) : IRequest<ImportSummaryDto>;

public record ImportSummaryDto(Guid BatchId, int Processed, int Created, int Updated, int Failed);

public class ImportCustomersCommandHandler(
    IFileParser parser,
    ICustomerRepository customerRepository,
    IImportLogRepository logRepository)
{
    public async Task<ImportSummaryDto> HandleAsync(ImportCustomersCommand command, CancellationToken ct = default)
    {
        var log = new ImportLog();
        await logRepository.AddAsync(log, ct);

        await foreach (var (lineNumber, rawLine, parsed, error) in parser.ParseStreamAsync(command.FileStream, ct))
        {
            if (error != null || parsed == null)
            {
                log.RecordFailure(lineNumber, rawLine, error ?? "Unknown parsing error");
                continue;
            }

            var existing = await customerRepository.GetByLegacyIdAsync(parsed.LegacyCustomerId, ct);

            if (existing != null)
            {
                existing.UpdateDetails(parsed.FullName, parsed.Email, parsed.SignupDate, parsed.Tier);
                log.RecordSuccess(isNew: false);
            }
            else
            {
                var customer = new Customer(parsed.LegacyCustomerId, parsed.FullName, parsed.Email, parsed.SignupDate, parsed.Tier);
                await customerRepository.AddAsync(customer, ct);
                log.RecordSuccess(isNew: true);
            }
        }

        log.Complete();
        await customerRepository.SaveChangesAsync(ct);
        await logRepository.SaveChangesAsync(ct);

        return new ImportSummaryDto(log.BatchId, log.TotalProcessed, log.CreatedCount, log.UpdatedCount, log.FailedCount);
    }
}
