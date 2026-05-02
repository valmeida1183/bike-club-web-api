using FluentValidation;

namespace BikeClub.Features.Address.Shared;

public class AddressRequestValidator<T> : AbstractValidator<T> where T : IAddressRequest
{
    protected AddressRequestValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street field is required")
            .MinimumLength(3).WithMessage("Street field must contain between 3 and 50 characters")
            .MaximumLength(50).WithMessage("Street field must contain between 3 and 50 characters");

        RuleFor(x => x.Complement)
            .NotEmpty().WithMessage("Complement field is required")
            .MaximumLength(50).WithMessage("Complement field must contain between 1 and 50 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State field is required")
            .Length(2, 2).WithMessage("State field must contain exactly 2 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City field is required")
            .MaximumLength(30).WithMessage("City field must contain between 1 and 30 characters");

        RuleFor(x => x.ZipCode)
            .GreaterThan(0).WithMessage("Zip Code field is required");
    }
}
