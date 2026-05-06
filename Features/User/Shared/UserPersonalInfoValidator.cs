using FluentValidation;

namespace BikeClub.Features.User.Shared;

public class UserPersonalInfoValidator<T> : AbstractValidator<T> where T : IUserPersonalInfoRequest
{
    protected UserPersonalInfoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email field is required")
            .EmailAddress().WithMessage("Email field is not a valid email")
            .Length(3, 100).WithMessage("Email field must contain between 3 and 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password field is required")
            .Length(6, 30).WithMessage("Password field must contain between 6 and 30 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone field is required")
            .Matches(@"^(?:\()[0-9]{2}(?:\))\s?[0-9]{4,5}(?:-)[0-9]{4}$")
                .WithMessage("Phone field is not a valid phone number")
            .Length(6, 20).WithMessage("Phone field must contain between 6 and 20 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required")
            .Length(1, 50).WithMessage("Name field must contain between 1 and 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName field is required")
            .Length(1, 50).WithMessage("LastName field must contain between 1 and 50 characters");
    }
}
