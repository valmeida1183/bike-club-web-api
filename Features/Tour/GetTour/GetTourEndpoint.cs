using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Tour.GetTour;

internal sealed class GetTourEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/tours", async (
                GetTourHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Tour");
}
