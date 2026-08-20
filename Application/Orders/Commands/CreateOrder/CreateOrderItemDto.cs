namespace Application.Orders.Commands.CreateOrder;

public record CreateOrderItemDto(
    string Description,
    string Sku,
    float UnitPrice,
    int Quantity
);
