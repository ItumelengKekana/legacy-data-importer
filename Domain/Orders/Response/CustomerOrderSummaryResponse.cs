namespace Domain.Orders.Response;

public record CustomerOrderSummaryResponse(
    int CustomerId,
    string LegacyCustomerId,
    string FullName,
    string Email,
    int TotalOrders,
    double TotalSpent
);
