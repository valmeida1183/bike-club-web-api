namespace BikeClub.Features.Bike.Shared;

public interface IBikeRequest
{
    int Gears { get; }
    decimal FrameSize { get; }
    decimal RimSize { get; }
    string? Model { get; }
    string? Description { get; }
    decimal Price { get; }
    string? Image { get; }
    string? GenderCode { get; }
    int CategoryId { get; }
}
