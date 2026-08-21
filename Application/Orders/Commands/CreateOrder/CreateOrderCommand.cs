using Mediator;

namespace Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(int? CustomerId,
    string? LegacyCustomerId,
    DateTime OrderDate,
    string Currency,
    string Status,
    List<CreateOrderItemDto> Items) : IRequest<CreateOrderResponse>;
