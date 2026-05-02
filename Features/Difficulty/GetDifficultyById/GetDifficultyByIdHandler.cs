using BikeClub.Data;
using BikeClub.Features.Difficulty.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Difficulty.GetDifficultyById;

internal sealed class GetDifficultyByIdHandler
{
    private readonly DataContext _db;

    public GetDifficultyByIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<DifficultyResponse>> Handle(int id, CancellationToken ct)
    {
        var difficulty = await _db.Difficulties
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (difficulty is null)
            return DifficultyErrors.NotFound;

        return DifficultyResponse.From(difficulty);
    }
}
