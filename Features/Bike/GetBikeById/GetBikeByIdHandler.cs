using BikeClub.Data;
using BikeClub.Features.Bike.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Bike.GetBikeById;

internal sealed class GetBikeByIdHandler
{
    private readonly DataContext _db;

    public GetBikeByIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<BikeResponse>> Handle(int id, CancellationToken ct)
    {
        var bike = await _db.Bikes
            .AsNoTracking()
            .Include(b => b.Category)
            .Include(b => b.Gender)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (bike is null)
            return BikeErrors.NotFound;

        return BikeResponse.From(bike);
    }
}
