using BikeClub.Data;
using BikeClub.Features.Difficulty.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DifficultyEntity = BikeClub.Domain.Entities.Difficulty;

namespace BikeClub.Features.Difficulty.UpdateDifficulty;

internal sealed class UpdateDifficultyHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdateDifficultyRequest> _validator;

    public UpdateDifficultyHandler(DataContext db, IValidator<UpdateDifficultyRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<DifficultyResponse>> Handle(int id, UpdateDifficultyRequest request, CancellationToken ct)
    {
        if (id != request.Id)
            return DifficultyErrors.IdMismatch;

        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<DifficultyResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var difficulty = new DifficultyEntity { Id = request.Id, Name = request.Name };

        _db.Entry(difficulty).State = EntityState.Modified;
        await _db.SaveChangesAsync(ct);

        return DifficultyResponse.From(difficulty);
    }
}
