using BikeClub.SharedKernel.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Infrastructure.ExceptionHandlers;

internal sealed class UniqueConstraintExceptionHandler : IExceptionHandler
{
    private const int SqlViolationOfUniqueIndex = 2601;
    private const int SqlViolationOfUniqueConstraint = 2627;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException dbUpdateException)
            return false;

        if (dbUpdateException.InnerException is not SqlException sqlException)
            return false;

        if (sqlException.Number != SqlViolationOfUniqueIndex &&
            sqlException.Number != SqlViolationOfUniqueConstraint)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(
            Result.Failure(InfrastructureErrors.UniqueConstraint),
            cancellationToken);

        return true;
    }
}
