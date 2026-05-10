using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Services;

namespace BikeClub.Extensions;

public static class ServicesExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICryptographerService, CryptographerService>();
        builder.Services.AddScoped<ITotalAmountCalculator, TotalAmountCalculator>();

        return builder;
    }
}
