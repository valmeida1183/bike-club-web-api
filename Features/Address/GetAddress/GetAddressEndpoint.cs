using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Address.GetAddress;

internal sealed class GetAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/addresses", async (
                GetAddressHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Address");
}
