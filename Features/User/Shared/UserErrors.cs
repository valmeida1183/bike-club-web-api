using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.User.Shared;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "The user with the specified ID was not found",
        ErrorType.NotFound);

    public static readonly Error IdMismatch = new(
        "User.IdMismatch",
        "Cannot change Id of User",
        ErrorType.Validation);
}
