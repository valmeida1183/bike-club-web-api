using BikeClub.Infrastructure.ExceptionHandlers;

namespace BikeClub.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<UniqueConstraintExceptionHandler>();
        services.AddExceptionHandler<ConcurrencyExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static WebApplication UseGlobalExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(_ => { });

        return app;
    }
}
