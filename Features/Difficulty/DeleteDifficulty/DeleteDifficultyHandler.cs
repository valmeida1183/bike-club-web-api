using BikeClub.Data;
using BikeClub.Features.Difficulty.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Difficulty.DeleteDifficulty;

internal sealed class DeleteDifficultyHandler
{
    private readonly DataContext _db;

    public DeleteDifficultyHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(int id, CancellationToken ct)
    {
        var difficulty = await _db.Difficulties.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (difficulty is null)
            return Result.Failure(DifficultyErrors.NotFound);

        _db.Difficulties.Remove(difficulty);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
