using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Role.UpdateRole;

internal sealed class UpdateRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("v1/roles/{name}", async (
                string name,
                UpdateRoleRequest request,
                UpdateRoleHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(name, request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Role");
}
