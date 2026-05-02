using CategoryEntity = BikeClub.Domain.Entities.Category;

namespace BikeClub.Features.Category.Shared;

public record CategoryResponse(int Id, string? Name)
{
    public static CategoryResponse From(CategoryEntity category) =>
        new(category.Id, category.Name);
}
