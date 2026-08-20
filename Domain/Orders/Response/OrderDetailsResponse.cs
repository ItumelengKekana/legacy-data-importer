namespace Domain.Orders.Response;

public record OrderDetailsResponse(
    Order? Order,
    string CustomerName,
    List<OrderItem> Items
);
