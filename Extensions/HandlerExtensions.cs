namespace BikeClub.Extensions;

public static class HandlerExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        var handlerTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract
                        && t.Namespace is not null
                        && t.Namespace.StartsWith("BikeClub.Features.")
                        && t.Name.EndsWith("Handler"));

        foreach (var handlerType in handlerTypes)
            services.AddScoped(handlerType);

        return services;
    }
}
