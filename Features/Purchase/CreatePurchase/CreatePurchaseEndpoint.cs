using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Purchase.CreatePurchase;

internal sealed class CreatePurchaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/purchases", async (
                CreatePurchaseRequest request,
                CreatePurchaseHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("Purchase");
}
