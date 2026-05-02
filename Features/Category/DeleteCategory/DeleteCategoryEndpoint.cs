using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Category.DeleteCategory;

internal sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("v1/categories/{id:int}", async (
                int id,
                DeleteCategoryHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Category");
}
