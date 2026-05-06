using BikeClub.Features.Bike.Shared;

namespace BikeClub.Features.Bike.CreateBike;

public record CreateBikeRequest(
    int Gears,
    decimal FrameSize,
    decimal RimSize,
    string? Model,
    string? Description,
    decimal Price,
    string? Image,
    string? GenderCode,
    int CategoryId) : IBikeRequest;
