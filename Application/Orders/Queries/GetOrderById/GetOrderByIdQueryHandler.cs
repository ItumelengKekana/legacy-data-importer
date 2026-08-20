using Domain.Orders;
using Mediator;

namespace Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>
{
    public async ValueTask<OrderDetailsDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetOrderDetailsByIdAsync(request.Id, cancellationToken);

        if (result is null)
        {
            return null;
        }

        var items = result.Items.ConvertAll(item => new OrderItemDto(
            item.Id,
            item.Description,
            item.Sku,
            (float)item.UnitPrice,
            item.Quantity,
            Math.Round((double)item.UnitPrice * item.Quantity, 2)
        ));

        double totalAmount = Math.Round(items.Sum(i => i.LineTotal), 2);

        return new OrderDetailsDto(
            result.Order!.Id,
            result.Order.CustomerId,
            result.CustomerName,
            result.Order.OrderDate,
            result.Order.Currency,
            result.Order.Status,
            items,
            totalAmount
        );
    }
}
