using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Gender.GetGender;

internal sealed class GetGenderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/genders", async (
                GetGenderHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .AllowAnonymous()
        .CacheOutput(b => b.Expire(TimeSpan.FromSeconds(30)).SetVaryByHeader("User-Agent"))
        .WithTags("Gender");
}
