using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.ShopCart.AddPurchaseToShopCart;

internal sealed class AddPurchaseToShopCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/shop-carts/add-purchase", async (
                AddPurchaseToShopCartRequest request,
                AddPurchaseToShopCartHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("ShopCart");
}
