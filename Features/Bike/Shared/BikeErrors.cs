using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Bike.Shared;

public static class BikeErrors
{
    public static readonly Error NotFound = new(
        "Bike.NotFound",
        "The bike with the specified ID was not found",
        ErrorType.NotFound);

    public static readonly Error IdMismatch = new(
        "Bike.IdMismatch",
        "Cannot change Id of Bike",
        ErrorType.Validation);
}
