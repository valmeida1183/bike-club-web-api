namespace BikeClub.Extensions;

public static class OutputCacheExtensions
{
    public static WebApplicationBuilder ConfigureOutputCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddOutputCache();
        return builder;
    }

    public static WebApplication UseDefaultOutputCache(this WebApplication app)
    {
        app.UseOutputCache();
        return app;
    }
}
