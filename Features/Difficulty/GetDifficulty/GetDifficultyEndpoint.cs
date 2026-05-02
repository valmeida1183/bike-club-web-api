using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Difficulty.GetDifficulty;

internal sealed class GetDifficultyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/difficulties", async (
                GetDifficultyHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .AllowAnonymous()
        .CacheOutput(b => b.Expire(TimeSpan.FromSeconds(30)).SetVaryByHeader("User-Agent"))
        .WithTags("Difficulty");
}
