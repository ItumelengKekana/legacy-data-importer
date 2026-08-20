using Domain.Orders;
using Mediator;

namespace Application.Orders.Queries.GetCustomerOrderSummary;

public class GetCustomerOrderSummaryQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetCustomerOrderSummaryQuery, List<CustomerOrderSummaryDto>>
{
    public async ValueTask<List<CustomerOrderSummaryDto>> Handle(
        GetCustomerOrderSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var summaries = await orderRepository.GetCustomerOrderSummariesAsync(
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return summaries.ConvertAll(s => new CustomerOrderSummaryDto(
            s.CustomerId,
            s.LegacyCustomerId,
            s.FullName,
            s.Email,
            s.TotalOrders,
            s.TotalSpent
        ));
    }
}
