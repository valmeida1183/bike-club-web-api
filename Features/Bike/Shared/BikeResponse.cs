using BikeClub.Features.Category.Shared;
using BikeClub.Features.Gender.Shared;
using BikeEntity = BikeClub.Domain.Entities.Bike;

namespace BikeClub.Features.Bike.Shared;

public record BikeResponse(
    int Id,
    int Gears,
    decimal FrameSize,
    decimal RimSize,
    string? Model,
    string? Description,
    decimal Price,
    string? Image,
    string? GenderCode,
    int CategoryId,
    GenderResponse? Gender,
    CategoryResponse? Category)
{
    public static BikeResponse From(BikeEntity bike) =>
        new(
            bike.Id,
            bike.Gears,
            bike.FrameSize,
            bike.RimSize,
            bike.Model,
            bike.Description,
            bike.Price,
            bike.Image,
            bike.GenderCode,
            bike.CategoryId,
            bike.Gender is not null ? GenderResponse.From(bike.Gender) : null,
            bike.Category is not null ? CategoryResponse.From(bike.Category) : null);
}
