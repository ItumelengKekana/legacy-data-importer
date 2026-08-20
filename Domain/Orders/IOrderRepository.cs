using Domain.Orders.Response;

namespace Domain.Orders;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order, IEnumerable<OrderItem> items, CancellationToken cancellationToken = default);
    Task<OrderDetailsResponse?> GetOrderDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<CustomerOrderSummaryResponse>> GetCustomerOrderSummariesAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
