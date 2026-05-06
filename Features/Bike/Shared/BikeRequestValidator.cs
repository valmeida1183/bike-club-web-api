using FluentValidation;

namespace BikeClub.Features.Bike.Shared;

public class BikeRequestValidator<T> : AbstractValidator<T> where T : IBikeRequest
{
    private const int MinGears = 0;
    private const int MaxGears = 36;
    private const decimal MinFrameSize = 13m;
    private const decimal MaxFrameSize = 24m;
    private const decimal MinRimSize = 12m;
    private const decimal MaxRimSize = 29m;
    private const int MaxModelLength = 20;
    private const int MaxDescriptionLength = 300;

    protected BikeRequestValidator()
    {
        RuleFor(x => x.Gears)
            .InclusiveBetween(MinGears, MaxGears)
            .WithMessage($"Gears field must contain a value between {MinGears} and {MaxGears}");

        RuleFor(x => x.FrameSize)
            .InclusiveBetween(MinFrameSize, MaxFrameSize)
            .WithMessage($"FrameSize field must contain a value between {MinFrameSize} and {MaxFrameSize}");

        RuleFor(x => x.RimSize)
            .InclusiveBetween(MinRimSize, MaxRimSize)
            .WithMessage($"RimSize field must contain a value between {MinRimSize} and {MaxRimSize}");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model field is required")
            .MaximumLength(MaxModelLength).WithMessage($"Model field must contain between 1 and {MaxModelLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description field is required")
            .MaximumLength(MaxDescriptionLength).WithMessage($"Description field must contain between 1 and {MaxDescriptionLength} characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Price field must be greater than or equal to 0");

        RuleFor(x => x.Image)
            .NotEmpty().WithMessage("Image field is required");
    }
}
