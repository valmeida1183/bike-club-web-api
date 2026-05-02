using BikeClub.Features.Category.Shared;

namespace BikeClub.Features.Category.UpdateCategory;

public record UpdateCategoryRequest(int Id, string? Name) : ICategoryRequest;
