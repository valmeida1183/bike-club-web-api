using FluentValidation;

namespace BikeClub.Features.Gender.CreateGender;

public class CreateGenderValidator : AbstractValidator<CreateGenderRequest>
{
    public CreateGenderValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code field is required")
            .MaximumLength(1).WithMessage("Code field must contain a maximum of 1 character");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description field is required")
            .MinimumLength(3).WithMessage("Description field must contain between 3 and 10 characters")
            .MaximumLength(10).WithMessage("Description field must contain between 3 and 10 characters");
    }
}
