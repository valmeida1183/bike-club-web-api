namespace BikeClub.Features.Bike.GetBike;

public record GetBikeRequest(
    int? CategoryId,
    string? GenderCode,
    decimal? Price,
    int? Gears,
    decimal? FrameSize,
    decimal? RimSize);
