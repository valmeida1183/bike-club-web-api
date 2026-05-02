using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Account.Shared;

public static class AccountErrors
{
    public static readonly Error InvalidCredentials = new(
        "Account.InvalidCredentials",
        "Invalid email or password",
        ErrorType.Unauthorized);

    public static readonly Error EmailAlreadyExists = new(
        "Account.EmailAlreadyExists",
        "An account with this email already exists",
        ErrorType.Conflict);
}
