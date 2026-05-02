using FluentValidation;

namespace BikeClub.Features.Account.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email field is required")
            .EmailAddress().WithMessage("Email field is not a valid email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password field is required");
    }
}
