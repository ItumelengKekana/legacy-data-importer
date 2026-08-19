using Application.Common.Interfaces;
using Domain.Customers;

namespace Infrastructure.Services;

public class FileParser : IFileParser
{
    private const int MinimumLineLength = 80;

    public async IAsyncEnumerable<(int LineNumber, string RawLine, ParsedCustomerDto? Customer, string? Error)> ParseStreamAsync(
        Stream stream,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        int lineNumber = 0;
        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.Length < MinimumLineLength)
            {
                yield return (lineNumber, line, null, $"Line length ({line.Length}) is shorter than expected {MinimumLineLength}.");
                continue;
            }

            List<string> fields =
            [
                line[0..10].Trim(),   // LegacyCustomerId
                line[10..40].Trim(),  // FullName
                line[40..70].Trim(),  // Email
                line[70..78].Trim(),  // SignupDate
                line[78..80].Trim()   // Tier
            ];

            if (string.IsNullOrWhiteSpace(fields[0]))
            {
                yield return (lineNumber, line, null, "LegacyCustomerId is missing.");
                continue;
            }

            if (!DateOnly.TryParseExact(fields[3], "yyyyMMdd", out var signupDate))
            {
                yield return (lineNumber, line, null, $"Invalid SignupDate format '{fields[3]}'. Expected YYYYMMDD.");
                continue;
            }

            if (!Enum.TryParse<Tier>(fields[4], out var tier))
            {
                yield return (lineNumber, line, null, $"Invalid Tier '{fields[4]}'. Expected A, B, or C.");
                continue;
            }

            var dto = new ParsedCustomerDto(fields[0], fields[1], fields[2], signupDate, tier);
            yield return (lineNumber, line, dto, null);
        }
    }
}
