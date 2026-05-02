using BikeClub.Data;
using BikeClub.Features.Address.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Address.GetAddressById;

internal sealed class GetAddressByIdHandler
{
    private readonly DataContext _db;

    public GetAddressByIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<AddressResponse>> Handle(int id, CancellationToken ct)
    {
        var address = await _db.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        if (address is null)
            return AddressErrors.NotFound;

        return AddressResponse.From(address);
    }
}
