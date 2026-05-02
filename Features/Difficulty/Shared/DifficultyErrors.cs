using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Difficulty.Shared;

public static class DifficultyErrors
{
    public static readonly Error NotFound = new(
        "Difficulty.NotFound",
        "The difficulty with the specified ID was not found",
        ErrorType.NotFound);

    public static readonly Error IdMismatch = new(
        "Difficulty.IdMismatch",
        "Cannot change Id of Difficulty",
        ErrorType.Validation);
}
