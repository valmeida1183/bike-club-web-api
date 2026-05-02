using BikeClub.Features.Address.Shared;

namespace BikeClub.Features.Address.UpdateAddress;

public record UpdateAddressRequest(int Id, string? Street, string? Complement, string? State, string? City, int ZipCode)
    : IAddressRequest;
