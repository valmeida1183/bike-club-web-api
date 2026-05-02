using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Category.GetCategory;

internal sealed class GetCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/categories", async (
                GetCategoryHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .AllowAnonymous()
        .CacheOutput(b => b.Expire(TimeSpan.FromSeconds(30)).SetVaryByHeader("User-Agent"))
        .WithTags("Category");
}
