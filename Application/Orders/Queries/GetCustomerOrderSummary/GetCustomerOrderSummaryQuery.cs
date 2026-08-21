using Mediator;

namespace Application.Orders.Queries.GetCustomerOrderSummary;

public record GetCustomerOrderSummaryQuery(
    string StartDate,
    string EndDate
) : IRequest<List<CustomerOrderSummaryDto>>;