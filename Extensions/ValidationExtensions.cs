using FluentValidation;

namespace BikeClub.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
