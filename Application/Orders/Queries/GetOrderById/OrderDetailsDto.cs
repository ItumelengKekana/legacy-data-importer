namespace Application.Orders.Queries.GetOrderById;

public record OrderDetailsDto(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    string Currency,
    string Status,
    List<OrderItemDto> Items,
    double TotalAmount
);