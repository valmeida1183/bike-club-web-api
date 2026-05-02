using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Role.CreateRole;

internal sealed class CreateRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/roles", async (
                CreateRoleRequest request,
                CreateRoleHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Role");
}
