using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Category.Shared;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound",
        "The category with the specified ID was not found",
        ErrorType.NotFound);

    public static readonly Error IdMismatch = new(
        "Category.IdMismatch",
        "Cannot change Id of Category",
        ErrorType.Validation);
}
