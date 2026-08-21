namespace Application.Orders.Queries.GetOrderById;

public record OrderItemDto(
    int Id,
    string Description,
    string Sku,
    float UnitPrice,
    int Quantity,
    double LineTotal
);
