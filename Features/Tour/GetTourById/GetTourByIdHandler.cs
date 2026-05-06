using BikeClub.Data;
using BikeClub.Features.Tour.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Tour.GetTourById;

internal sealed class GetTourByIdHandler
{
    private readonly DataContext _db;

    public GetTourByIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<TourResponse>> Handle(int id, CancellationToken ct)
    {
        var tour = await _db.Tours
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (tour is null)
            return TourErrors.NotFound;

        return TourResponse.From(tour);
    }
}
