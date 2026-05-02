using BikeClub.Data;
using BikeClub.Features.Address.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Address.GetAddress;

internal sealed class GetAddressHandler
{
    private readonly DataContext _db;

    public GetAddressHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<AddressResponse>>> Handle(CancellationToken ct)
    {
        var addresses = await _db.Addresses
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<AddressResponse>>(
            addresses.Select(AddressResponse.From).ToList());
    }
}
