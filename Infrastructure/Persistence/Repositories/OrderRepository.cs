using Domain.Orders;
using Domain.Orders.Response;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task AddOrderAsync(Order order, IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Assign foreign key to items
        foreach (var item in items)
        {
            item.OrderId = order.Id;
        }

        await dbContext.OrderItems.AddRangeAsync(items, cancellationToken);
    }

    public async Task<OrderDetailsResponse?> GetOrderDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var orderData = await (
            from o in dbContext.Orders
            join c in dbContext.Customers on o.CustomerId equals c.Id
            where o.Id == id
            select new { Order = o, CustomerName = c.FullName }
        ).FirstOrDefaultAsync(cancellationToken);

        if (orderData is null) return null;

        var items = await dbContext.OrderItems
            .Where(oi => oi.OrderId == id)
            .ToListAsync(cancellationToken);

        return new OrderDetailsResponse(orderData.Order, orderData.CustomerName, items);
    }

    public async Task<List<CustomerOrderSummaryResponse>> GetCustomerOrderSummariesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .Select(c => new
            {
                c.Id,
                c.LegacyCustomerId,
                c.FullName,
                c.Email,
                Orders = dbContext.Orders.Where(o => o.CustomerId == c.Id
                                                  && o.OrderDate >= fromDate
                                                  && o.OrderDate <= toDate)
            })
            .Where(c => c.Orders.Any())
            .Select(c => new CustomerOrderSummaryResponse(
                c.Id,
                c.LegacyCustomerId,
                c.FullName,
                c.Email,
                c.Orders.Count(),
                Math.Round(
                    (double)dbContext.OrderItems
                        .Where(oi => c.Orders.Select(o => o.Id).Contains(oi.OrderId))
                        .Sum(oi => oi.UnitPrice * oi.Quantity),
                    2
                )
            ))
            .OrderByDescending(s => s.TotalSpent)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
