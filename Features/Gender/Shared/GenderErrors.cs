using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Gender.Shared;

public static class GenderErrors
{
    public static readonly Error NotFound = new(
        "Gender.NotFound",
        "The gender with the specified code was not found",
        ErrorType.NotFound);
}
