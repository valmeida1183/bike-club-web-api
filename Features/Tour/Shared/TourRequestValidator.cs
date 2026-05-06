using FluentValidation;

namespace BikeClub.Features.Tour.Shared;

public class TourRequestValidator<T> : AbstractValidator<T> where T : ITourRequest
{
    protected TourRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate field is required");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate field is required");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description field is required")
            .MinimumLength(3).WithMessage("Description field must be at least 3 characters")
            .MaximumLength(300).WithMessage("Description field must not exceed 300 characters");
    }
}
