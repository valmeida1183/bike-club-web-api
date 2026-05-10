using BikeEntity = BikeClub.Domain.Entities.Bike;

namespace BikeClub.Features.ShopCart.Shared;

public record BikeInCartResponse(
    int Id,
    int Gears,
    decimal FrameSize,
    decimal RimSize,
    string? Model,
    string? Description,
    decimal Price,
    string? Image,
    string? GenderCode,
    int CategoryId)
{
    public static BikeInCartResponse From(BikeEntity bike) =>
        new(bike.Id, bike.Gears, bike.FrameSize, bike.RimSize,
            bike.Model, bike.Description, bike.Price, bike.Image,
            bike.GenderCode, bike.CategoryId);
}
