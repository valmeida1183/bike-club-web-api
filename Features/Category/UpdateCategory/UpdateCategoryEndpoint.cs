using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Category.UpdateCategory;

internal sealed class UpdateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("v1/categories/{id:int}", async (
                int id,
                UpdateCategoryRequest request,
                UpdateCategoryHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Category");
}
