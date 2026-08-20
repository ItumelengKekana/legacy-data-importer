using FluentValidation;

namespace Application.Orders.Queries.GetCustomerOrderSummary;

public class GetCustomerOrderSummaryQueryValidator : AbstractValidator<GetCustomerOrderSummaryQuery>
{
    public GetCustomerOrderSummaryQueryValidator()
    {
        RuleFor(x => x.FromDate)
            .NotEmpty()
            .WithMessage("Start Date is required.");

        RuleFor(x => x.ToDate)
            .NotEmpty()
            .WithMessage("End Date is required.")
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("End Date must be greater than or equal to Start Date.");
    }
}
