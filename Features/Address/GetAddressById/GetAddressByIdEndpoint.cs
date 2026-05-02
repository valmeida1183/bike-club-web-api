using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Address.GetAddressById;

internal sealed class GetAddressByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/addresses/{id:int}", async (
                int id,
                GetAddressByIdHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Address");
}
