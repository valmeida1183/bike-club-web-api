using AddressEntity = BikeClub.Domain.Entities.Address;

namespace BikeClub.Features.Address.Shared;

public record AddressResponse(int Id, string? Street, string? Complement, string? State, string? City, int ZipCode)
{
    public static AddressResponse From(AddressEntity address) =>
        new(address.Id, address.Street, address.Complement, address.State, address.City, address.ZipCode);
}
