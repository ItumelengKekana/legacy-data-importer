using Mediator;

namespace Application.Orders.Queries.GetCustomerOrderSummary;

public record GetCustomerOrderSummaryQuery(
    DateTime FromDate,
    DateTime ToDate
) : IRequest<List<CustomerOrderSummaryDto>>;