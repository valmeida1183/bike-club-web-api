using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Role.GetRoleByName;

internal sealed class GetRoleByNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/roles/{name}", async (
                string name,
                GetRoleByNameHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(name, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Role");
}
