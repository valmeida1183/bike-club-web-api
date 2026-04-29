using BikeClub.SharedKernel.Results;

namespace BikeClub.Infrastructure.ExceptionHandlers;

public static class InfrastructureErrors
{
    public static readonly Error UniqueConstraint = new(
        "Db.UniqueConstraint",
        "Cannot create a record that already exists.",
        ErrorType.Conflict);

    public static readonly Error Concurrency = new(
        "Db.Concurrency",
        "This record is already updated.",
        ErrorType.Validation);

    public static readonly Error Unexpected = new(
        "Server.Unexpected",
        "An unexpected error occurred.",
        ErrorType.Failure);
}
