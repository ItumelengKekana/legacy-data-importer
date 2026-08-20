using Domain.Customers;
using Domain.Orders;
using Mediator;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository) : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async ValueTask<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateOrderCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count != 0)
        {
            List<string> messages = [];
            List<string> names = [];
            var paramExists = false;

            foreach (var item in validationResult.Errors)
            {
                messages.Add(item.ErrorMessage);

                var _tag = item.FormattedMessagePlaceholderValues;
                var name = _tag.FirstOrDefault(t => t.Key == "PropertyName");

                names.Add(name.Value.ToString()!);
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
                return new CreateOrderResponse(false, string.Join(", ", messages));
            }
        }

        int resolvedCustomerId;

        if (request.CustomerId.HasValue)
        {
            resolvedCustomerId = request.CustomerId.Value;
        }
        else if (!string.IsNullOrWhiteSpace(request.LegacyCustomerId))
        {
            var customer = await customerRepository.GetByLegacyIdAsync(request.LegacyCustomerId, cancellationToken);
            if (customer is null)
            {
                return new CreateOrderResponse(false, $"Customer with Legacy ID '{request.LegacyCustomerId}' was not found.");
            }

            resolvedCustomerId = customer.Id;
        }
        else
        {
            return new CreateOrderResponse(false, "Either CustomerId or LegacyCustomerId must be supplied.");
        }

        var order = new Order
        {
            CustomerId = resolvedCustomerId,
            OrderDate = request.OrderDate,
            Currency = request.Currency,
            Status = request.Status
        };

        var items = request.Items.Select(item => new OrderItem
        {
            Description = item.Description,
            Sku = item.Sku,
            UnitPrice = (decimal)item.UnitPrice,
            Quantity = item.Quantity
        }).ToList();

        await orderRepository.AddOrderAsync(order, items, cancellationToken);

        return new CreateOrderResponse(true, $"Order {order.Id} was created successfully.");
    }
}
