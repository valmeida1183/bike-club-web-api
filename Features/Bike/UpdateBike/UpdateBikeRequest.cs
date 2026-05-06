using BikeClub.Features.Bike.Shared;

namespace BikeClub.Features.Bike.UpdateBike;

public record UpdateBikeRequest(
    int Id,
    int Gears,
    decimal FrameSize,
    decimal RimSize,
    string? Model,
    string? Description,
    decimal Price,
    string? Image,
    string? GenderCode,
    int CategoryId) : IBikeRequest;
