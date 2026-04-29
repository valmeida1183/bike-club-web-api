using BikeClub.SharedKernel.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Infrastructure.ExceptionHandlers;

internal sealed class ConcurrencyExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateConcurrencyException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            Result.Failure(InfrastructureErrors.Concurrency),
            cancellationToken);

        return true;
    }
}
