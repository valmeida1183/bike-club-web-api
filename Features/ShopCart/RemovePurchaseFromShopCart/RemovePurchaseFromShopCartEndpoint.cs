using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.ShopCart.RemovePurchaseFromShopCart;

internal sealed class RemovePurchaseFromShopCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("v1/shop-carts/remove-purchase/{shopCartId:int}/{bikeId:int}", async (
                int shopCartId,
                int bikeId,
                RemovePurchaseFromShopCartHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(shopCartId, bikeId, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("ShopCart");
}
