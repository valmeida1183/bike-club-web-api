using BikeClub.Features.Address.Shared;

namespace BikeClub.Features.Address.CreateAddress;

public record CreateAddressRequest(string? Street, string? Complement, string? State, string? City, int ZipCode)
    : IAddressRequest;
