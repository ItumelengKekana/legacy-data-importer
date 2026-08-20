using Mediator;

namespace Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDetailsDto?>;
