using FluentValidation;

namespace Application.Customers.Commands.ImportCustomers;

public class ImportCustomersCommandValidator : AbstractValidator<ImportCustomersCommand>
{
    public ImportCustomersCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .NotEmpty().WithMessage("Please make sure the uploaded file is not empty");
    }
}
