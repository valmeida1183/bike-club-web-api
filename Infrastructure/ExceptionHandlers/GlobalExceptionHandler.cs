using BikeClub.SharedKernel.Results;
using Microsoft.AspNetCore.Diagnostics;

namespace BikeClub.Infrastructure.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            Result.Failure(InfrastructureErrors.Unexpected),
            cancellationToken);

        return true;
    }
}
