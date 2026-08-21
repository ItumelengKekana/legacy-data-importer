using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Customers;
using Domain.Imports;
using Mediator;

namespace Application.Customers.Commands.ImportCustomers;

public class ImportCustomersCommandHandler(
    IFileParser parser,
    ICustomerRepository customerRepository,
    IImportLogRepository logRepository) : IRequestHandler<ImportCustomersCommand, ImportSummaryDto>
{
    public async ValueTask<ImportSummaryDto> Handle(ImportCustomersCommand command, CancellationToken ct = default)
    {
        var validator = new ImportCustomersCommandValidator();
        var validationResult = await validator.ValidateAsync(command, ct);

        if (validationResult.Errors.Count != 0)
        {
            List<string> messages = [];
            List<string> names = [];
            var paramExists = false;

            foreach (var item in validationResult.Errors)
            {
                messages.Add(item.ErrorMessage);

                var _tag = item;
                var name = _tag.PropertyName;

                names.Add(name);
            }

            foreach (var item in names)
            {
                var check = messages.Any(x => x.Contains(item));
                if (check)
                {
                    paramExists = true;
                }
            }

            if (paramExists)
            {
                throw new BadRequestException(string.Join(", ", messages));
            }
        }

        var log = new ImportLog();
        await logRepository.AddAsync(log, ct);

        if (command.File is null || command.File.Length == 0)
        {
            throw new BadRequestException("A valid non-empty file is required.");
        }

        using var stream = command.File.OpenReadStream();
        await foreach (var (lineNumber, rawLine, parsed, error) in parser.ParseStreamAsync(stream, ct))
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
