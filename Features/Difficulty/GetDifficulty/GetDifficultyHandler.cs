using BikeClub.Data;
using BikeClub.Features.Difficulty.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Difficulty.GetDifficulty;

internal sealed class GetDifficultyHandler
{
    private readonly DataContext _db;

    public GetDifficultyHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<DifficultyResponse>>> Handle(CancellationToken ct)
    {
        var difficulties = await _db.Difficulties
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<DifficultyResponse>>(
            difficulties.Select(DifficultyResponse.From).ToList());
    }
}
