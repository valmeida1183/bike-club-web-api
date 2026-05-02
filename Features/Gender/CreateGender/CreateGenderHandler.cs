using BikeClub.Data;
using BikeClub.Features.Gender.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using GenderEntity = BikeClub.Domain.Entities.Gender;

namespace BikeClub.Features.Gender.CreateGender;

internal sealed class CreateGenderHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateGenderRequest> _validator;

    public CreateGenderHandler(DataContext db, IValidator<CreateGenderRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<GenderResponse>> Handle(CreateGenderRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<GenderResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var gender = new GenderEntity { Code = request.Code, Description = request.Description };

        _db.Genders.Add(gender);
        await _db.SaveChangesAsync(ct);

        return GenderResponse.From(gender);
    }
}
