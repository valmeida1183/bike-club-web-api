using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Bike.UpdateBike;

internal sealed class UpdateBikeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("v1/bikes/{id:int}", async (
                int id,
                UpdateBikeRequest request,
                UpdateBikeHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Bike");
}
