using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Tour.Shared;

public static class TourErrors
{
    public static readonly Error NotFound = new(
        "Tour.NotFound",
        "The tour with the specified ID was not found",
        ErrorType.NotFound);

    public static readonly Error IdMismatch = new(
        "Tour.IdMismatch",
        "Cannot change Id of Tour",
        ErrorType.Validation);
}
