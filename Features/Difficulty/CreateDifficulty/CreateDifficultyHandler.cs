using BikeClub.Data;
using BikeClub.Features.Difficulty.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using DifficultyEntity = BikeClub.Domain.Entities.Difficulty;

namespace BikeClub.Features.Difficulty.CreateDifficulty;

internal sealed class CreateDifficultyHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateDifficultyRequest> _validator;

    public CreateDifficultyHandler(DataContext db, IValidator<CreateDifficultyRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<DifficultyResponse>> Handle(CreateDifficultyRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<DifficultyResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var difficulty = new DifficultyEntity { Name = request.Name };

        _db.Difficulties.Add(difficulty);
        await _db.SaveChangesAsync(ct);

        return DifficultyResponse.From(difficulty);
    }
}
