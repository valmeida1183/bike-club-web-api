namespace BikeClub.Extensions;

public static class CorsExtensions
{
    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors();
        return builder;
    }

    public static WebApplication UseDefaultCors(this WebApplication app)
    {
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        return app;
    }
}
