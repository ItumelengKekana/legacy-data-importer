namespace Application.Customers.Commands.ImportCustomers;

public record ImportSummaryDto(Guid BatchId, int Processed, int Created, int Updated, int Failed);
