using BikeClub.Data;
using BikeClub.Features.Gender.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Gender.GetGenderByCode;

internal sealed class GetGenderByCodeHandler
{
    private readonly DataContext _db;

    public GetGenderByCodeHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<GenderResponse>> Handle(string code, CancellationToken ct)
    {
        var gender = await _db.Genders
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Code == code, ct);

        if (gender is null)
            return GenderErrors.NotFound;

        return GenderResponse.From(gender);
    }
}
