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

        await dbContext.Entry(order).ReloadAsync(cancellationToken);
        // Assign foreign key to items
        foreach (var item in items)
        {
            item.OrderId = order.Id;
        }

        await dbContext.OrderItems.AddRangeAsync(items, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
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
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        // Load customer and order data into memory
        var customerOrders = await dbContext.Customers
            .Select(c => new
            {
                c.Id,
                c.LegacyCustomerId,
                c.FullName,
                c.Email,
                Orders = dbContext.Orders
                    .Where(o => o.CustomerId == c.Id
                                && o.OrderDate >= startDate
                                && o.OrderDate <= endDate)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        // Load order items into memory (for the date range)
        var orderIds = customerOrders
            .SelectMany(c => c.Orders.Select(o => o.Id))
            .Distinct()
            .ToList();

        var orderItemsDict = await dbContext.OrderItems
            .Where(oi => orderIds.Contains(oi.OrderId))
            .GroupBy(oi => oi.OrderId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.AsEnumerable().Sum(oi => oi.UnitPrice * oi.Quantity),
                cancellationToken);

        // Perform aggregations in memory
        return customerOrders
            .Where(c => c.Orders.Count != 0)
            .Select(c => new CustomerOrderSummaryResponse(
                c.Id,
                c.LegacyCustomerId,
                c.FullName,
                c.Email,
                c.Orders.Count,
                Math.Round(
                    (double)c.Orders
                        .Sum(o => orderItemsDict.GetValueOrDefault(o.Id, 0m)),
                    2)
            ))
            .OrderByDescending(s => s.TotalSpent)
            .ToList();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
