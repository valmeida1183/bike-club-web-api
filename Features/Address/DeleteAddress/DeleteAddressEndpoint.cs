using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Address.DeleteAddress;

internal sealed class DeleteAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("v1/addresses/{id:int}", async (
                int id,
                DeleteAddressHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Address");
}
