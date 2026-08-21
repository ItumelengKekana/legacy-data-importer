using FluentValidation;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
	public CreateOrderCommandValidator()
	{
		RuleFor(x => x)
			.Must(x => x.CustomerId.HasValue || !string.IsNullOrWhiteSpace(x.LegacyCustomerId))
			.WithMessage("Either CustomerId or LegacyCustomerId must be supplied.");

		When(x => x.CustomerId.HasValue, () =>
		{
			RuleFor(x => x.CustomerId!.Value)
				.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
		});

		When(x => !string.IsNullOrWhiteSpace(x.LegacyCustomerId), () =>
		{
			RuleFor(x => x.LegacyCustomerId)
				.MaximumLength(10).WithMessage("{PropertyName} cannot exceed 10 characters.");
		});

		RuleFor(x => x.OrderDate)
			.NotEmpty().WithMessage("{PropertyName} is required.");

		RuleFor(x => x.Currency)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.Length(3).WithMessage("{PropertyName} must be exactly 3 characters (e.g., USD, ZAR).");

		RuleFor(x => x.Status)
			.NotEmpty().WithMessage("Status is required.")
			.MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

		RuleFor(x => x.Items)
			.NotNull().WithMessage("{PropertyName} collection cannot be null.")
			.Must(items => items?.Count > 0)
			.WithMessage("{PropertyName} must contain at least one item.");

		RuleForEach(x => x.Items)
			.SetValidator(new CreateOrderItemDtoValidator());
	}
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
	public CreateOrderItemDtoValidator()
	{
		RuleFor(x => x.Description)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");

		RuleFor(x => x.Sku)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

		RuleFor(x => x.UnitPrice)
			.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

		RuleFor(x => x.Quantity)
			.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
	}
}