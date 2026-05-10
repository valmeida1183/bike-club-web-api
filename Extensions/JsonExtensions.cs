using System.Text.Json.Serialization;

namespace BikeClub.Extensions;

public static class JsonExtensions
{
    public static WebApplicationBuilder ConfigureJson(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        return builder;
    }
}
