using Microsoft.AspNetCore.ResponseCompression;

namespace BikeClub.Extensions;

public static class CompressionExtensions
{
    public static WebApplicationBuilder ConfigureCompression(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(opt =>
        {
            opt.EnableForHttps = true;
            opt.Providers.Add<BrotliCompressionProvider>();
            opt.MimeTypes = ResponseCompressionDefaults.MimeTypes
                .Concat(new[] { "application/json; charset=utf-8" });
        });

        return builder;
    }
}
