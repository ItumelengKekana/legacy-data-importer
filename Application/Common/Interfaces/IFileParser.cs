using Domain.Customers;

namespace Application.Common.Interfaces;

public record ParsedCustomerDto(string LegacyCustomerId, string FullName, string Email, DateOnly SignupDate, Tier Tier);

public interface IFileParser
{
    IAsyncEnumerable<(int LineNumber, string RawLine, ParsedCustomerDto? Customer, string? Error)> ParseStreamAsync(
        Stream stream,
        CancellationToken cancellationToken = default);
}
