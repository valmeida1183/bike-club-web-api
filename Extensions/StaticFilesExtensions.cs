using Microsoft.Extensions.FileProviders;

namespace BikeClub.Extensions;

public static class StaticFilesExtensions
{
    public static WebApplication UseResourceStaticFiles(this WebApplication app)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Resources")),
            RequestPath = "/Resources"
        });

        return app;
    }
}
