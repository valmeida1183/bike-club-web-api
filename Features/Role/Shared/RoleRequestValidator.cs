using FluentValidation;

namespace BikeClub.Features.Role.Shared;

public class RoleRequestValidator<T> : AbstractValidator<T> where T : IRoleRequest
{
    protected RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required")
            .MinimumLength(3).WithMessage("Name field must contain between 3 and 20 characters")
            .MaximumLength(20).WithMessage("Name field must contain between 3 and 20 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description field is required")
            .MinimumLength(3).WithMessage("Description field must contain between 3 and 50 characters")
            .MaximumLength(50).WithMessage("Description field must contain between 3 and 50 characters");
    }
}
