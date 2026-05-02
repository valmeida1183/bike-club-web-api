using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Category.GetCategoryById;

internal sealed class GetCategoryByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/categories/{id:int}", async (
                int id,
                GetCategoryByIdHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .AllowAnonymous()
        .WithTags("Category");
}
