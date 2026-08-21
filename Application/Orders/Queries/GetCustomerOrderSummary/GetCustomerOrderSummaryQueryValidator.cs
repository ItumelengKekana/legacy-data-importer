using FluentValidation;
using System.Globalization;

namespace Application.Orders.Queries.GetCustomerOrderSummary;

public class GetCustomerOrderSummaryQueryValidator : AbstractValidator<GetCustomerOrderSummaryQuery>
{
	private const string DateFormat = "yyyy-MM-dd";

	public GetCustomerOrderSummaryQueryValidator()
	{
		RuleFor(x => x.StartDate)
			.NotEmpty()
			.WithMessage("StartDate is required.")
			.Matches(@"^\d{4}-\d{2}-\d{2}$")
			.WithMessage($"StartDate must be in {DateFormat} format.")
			.Custom(ValidateDateFormat);

		RuleFor(x => x.EndDate)
			.NotEmpty()
			.WithMessage("EndDate is required.")
			.Matches(@"^\d{4}-\d{2}-\d{2}$")
			.WithMessage($"EndDate must be in {DateFormat} format.")
			.Custom(ValidateDateFormat);

		// Compare parsed dates
		RuleFor(x => x.StartDate)
			.Custom((startDate, context) =>
			{
				var endDate = context.InstanceToValidate.EndDate;

				if (DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) &&
					DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
				{
					if (start > end)
					{
						context.AddFailure("StartDate must be less than or equal to EndDate.");
					}
				}
			});
	}

	private void ValidateDateFormat(string dateString, ValidationContext<GetCustomerOrderSummaryQuery> context)
	{
		if (!DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
		{
			var fieldName = context.PropertyPath?.Contains("Start") ?? false ? "Start Date" : "End Date";
			context.AddFailure($"{fieldName} must be a valid date in {DateFormat} format.");
		}
	}
}
