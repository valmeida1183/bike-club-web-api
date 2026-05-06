using BikeClub.Data;
using BikeClub.Features.Tour.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Tour.GetTour;

internal sealed class GetTourHandler
{
    private readonly DataContext _db;

    public GetTourHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<TourResponse>>> Handle(CancellationToken ct)
    {
        var tours = await _db.Tours
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<TourResponse>>(
            tours.Select(TourResponse.From).ToList());
    }
}
