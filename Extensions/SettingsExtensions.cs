using BikeClub.SharedKernel;

namespace BikeClub.Extensions;

public static class SettingsExtensions
{
    public static WebApplication LoadSettings(this WebApplication app)
    {
        Settings.Secret = app.Configuration.GetValue<string>("Secret");
        Settings.TokenExpirationHours = app.Configuration.GetValue<int>("TokenExpirationHours");
        return app;
    }
}
