using Mediator;

namespace Application.Customers.Commands.ImportCustomers;

public record ImportCustomersCommand(Stream FileStream) : IRequest<ImportSummaryDto>;
