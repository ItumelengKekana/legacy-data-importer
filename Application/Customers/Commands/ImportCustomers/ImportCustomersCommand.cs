using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Customers.Commands.ImportCustomers;

public record ImportCustomersCommand(IFormFile File) : IRequest<ImportSummaryDto>;
