using Microsoft.OpenApi;

namespace BikeClub.Extensions;

public static class SwaggerExtensions
{
    public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Bike Club Api",
                Version = "v1"
            });
        });

        return builder;
    }

    public static WebApplication UseSwaggerUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bike Club Api Version: 1");
        });

        return app;
    }
}
