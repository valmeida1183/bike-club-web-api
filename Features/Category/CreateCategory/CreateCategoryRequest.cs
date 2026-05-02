using BikeClub.Features.Category.Shared;

namespace BikeClub.Features.Category.CreateCategory;

public record CreateCategoryRequest(string? Name) : ICategoryRequest;
