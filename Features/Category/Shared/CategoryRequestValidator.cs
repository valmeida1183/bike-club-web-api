using FluentValidation;

namespace BikeClub.Features.Category.Shared;

public class CategoryRequestValidator<T> : AbstractValidator<T> where T : ICategoryRequest
{
    protected CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required")
            .MinimumLength(3).WithMessage("Name field must contain between 3 and 15 characters")
            .MaximumLength(15).WithMessage("Name field must contain between 3 and 15 characters");
    }
}
