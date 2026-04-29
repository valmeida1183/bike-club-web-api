using System.Text.Json.Serialization;
using BikeClub.SharedKernel.Services;

namespace BikeClub.Extensions;

public static class ServicesExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICryptographerService, CryptographerService>();

        return builder;
    }
}
