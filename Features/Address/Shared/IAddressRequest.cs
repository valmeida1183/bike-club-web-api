namespace BikeClub.Features.Address.Shared;

public interface IAddressRequest
{
    string? Street { get; }
    string? Complement { get; }
    string? State { get; }
    string? City { get; }
    int ZipCode { get; }
}
