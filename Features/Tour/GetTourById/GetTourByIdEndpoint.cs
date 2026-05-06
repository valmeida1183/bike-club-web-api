using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Tour.GetTourById;

internal sealed class GetTourByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/tours/{id:int}", async (
                int id,
                GetTourByIdHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Tour");
}
