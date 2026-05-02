using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.Role.Shared;

public static class RoleErrors
{
    public static readonly Error NotFound = new(
        "Role.NotFound",
        "The role with the specified name was not found",
        ErrorType.NotFound);

    public static readonly Error NameMismatch = new(
        "Role.NameMismatch",
        "Cannot change name of Role",
        ErrorType.Validation);
}
