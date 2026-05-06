using BikeClub.Data;
using BikeClub.Features.Tour.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Tour.DeleteTour;

internal sealed class DeleteTourHandler
{
    private readonly DataContext _db;

    public DeleteTourHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(int id, CancellationToken ct)
    {
        var tour = await _db.Tours.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tour is null)
            return Result.Failure(TourErrors.NotFound);

        _db.Tours.Remove(tour);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
