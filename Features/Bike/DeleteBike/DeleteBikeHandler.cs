using BikeClub.Data;
using BikeClub.Features.Bike.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Bike.DeleteBike;

internal sealed class DeleteBikeHandler
{
    private readonly DataContext _db;

    public DeleteBikeHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(int id, CancellationToken ct)
    {
        var bike = await _db.Bikes.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (bike is null)
            return Result.Failure(BikeErrors.NotFound);

        _db.Bikes.Remove(bike);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
