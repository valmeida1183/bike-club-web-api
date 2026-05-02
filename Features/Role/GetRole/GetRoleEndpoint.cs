using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Role.GetRole;

internal sealed class GetRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/roles", async (
                GetRoleHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Role");
}
