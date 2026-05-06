using BikeClub.Data;
using BikeClub.Features.Bike.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Bike.GetBike;

internal sealed class GetBikeHandler
{
    private readonly DataContext _db;

    public GetBikeHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<BikeResponse>>> Handle(GetBikeRequest request, CancellationToken ct)
    {
        var bikes = await _db.Bikes
            .AsNoTracking()
            .Where(b => (!request.CategoryId.HasValue || b.CategoryId == request.CategoryId) &&
                        (string.IsNullOrEmpty(request.GenderCode) || b.GenderCode == request.GenderCode) &&
                        (!request.Price.HasValue || b.Price <= request.Price) &&
                        (!request.Gears.HasValue || b.Gears <= request.Gears) &&
                        (!request.FrameSize.HasValue || b.FrameSize <= request.FrameSize) &&
                        (!request.RimSize.HasValue || b.RimSize <= request.RimSize))
            .Include(b => b.Gender)
            .Include(b => b.Category)
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<BikeResponse>>(
            bikes.Select(BikeResponse.From).ToList());
    }
}
