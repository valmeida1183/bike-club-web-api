using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Address.UpdateAddress;

internal sealed class UpdateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("v1/addresses/{id:int}", async (
                int id,
                UpdateAddressRequest request,
                UpdateAddressHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Address");
}
