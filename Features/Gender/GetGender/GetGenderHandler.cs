using BikeClub.Data;
using BikeClub.Features.Gender.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Gender.GetGender;

internal sealed class GetGenderHandler
{
    private readonly DataContext _db;

    public GetGenderHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<GenderResponse>>> Handle(CancellationToken ct)
    {
        var genders = await _db.Genders
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<GenderResponse>>(
            genders.Select(GenderResponse.From).ToList());
    }
}
