using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Bike.GetBike;

internal sealed class GetBikeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/bikes", async (
                [AsParameters] GetBikeRequest request,
                GetBikeHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Bike");
}
