namespace Application.Orders.Queries.GetCustomerOrderSummary;

public record CustomerOrderSummaryDto(
    int CustomerId,
    string LegacyCustomerId,
    string FullName,
    string Email,
    int TotalOrders,
    double TotalSpent
);
