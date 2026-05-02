using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Address.Shared;

public static class AddressErrors
{
    public static readonly Error NotFound = new(
        "Address.NotFound",
        "The address with the specified ID was not found",
        ErrorType.NotFound);

    public static readonly Error IdMismatch = new(
        "Address.IdMismatch",
        "Cannot change Id of Address",
        ErrorType.Validation);
}
