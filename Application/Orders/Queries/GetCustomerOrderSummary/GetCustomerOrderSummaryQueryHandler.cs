using Application.Common.Exceptions;
using Domain.Orders;
using Mediator;
using System.Globalization;

namespace Application.Orders.Queries.GetCustomerOrderSummary;

public class GetCustomerOrderSummaryQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetCustomerOrderSummaryQuery, List<CustomerOrderSummaryDto>>
{
    public async ValueTask<List<CustomerOrderSummaryDto>> Handle(
        GetCustomerOrderSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var startDate = DateTime.ParseExact(request.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endDate = DateTime.ParseExact(request.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var validator = new GetCustomerOrderSummaryQueryValidator();
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

        var summaries = await orderRepository.GetCustomerOrderSummariesAsync(
            startDate,
            endDate,
            cancellationToken);

        if (summaries.Count == 0)
        {
            throw new NotFoundMessageException("No orders found for the specified date range.");
        }

        return summaries.ConvertAll(s => new CustomerOrderSummaryDto(
            s.CustomerId,
            s.LegacyCustomerId,
            s.FullName,
            s.Email,
            s.TotalOrders,
            s.TotalSpent
        ));
    }
}
