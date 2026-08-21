using Application.Common.Exceptions;
using Domain.Orders;
using Mediator;

namespace Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>
{
    public async ValueTask<OrderDetailsDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetOrderByIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count != 0)
        {
            List<string> messages = [];
            List<string> names = [];
            var paramExists = false;

            foreach (var item in validationResult.Errors)
            {
                messages.Add(item.ErrorMessage);

                var _tag = item;
                var name = _tag.PropertyName;

                names.Add(name);
            }

            foreach (var item in names)
            {
                var check = messages.Any(x => x.Contains(item));
                if (check)
                {
                    paramExists = true;
                }
            }

            if (paramExists)
            {
                throw new BadRequestException(string.Join(", ", messages));
            }
        }

        var result = await orderRepository.GetOrderDetailsByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundMessageException("No order was found for the specified Id");

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
