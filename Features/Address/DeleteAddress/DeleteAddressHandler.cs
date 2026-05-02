using BikeClub.Data;
using BikeClub.Features.Address.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Address.DeleteAddress;

internal sealed class DeleteAddressHandler
{
    private readonly DataContext _db;

    public DeleteAddressHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(int id, CancellationToken ct)
    {
        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (address is null)
            return Result.Failure(AddressErrors.NotFound);

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
