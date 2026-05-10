using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.ShopCart.UpdatePurchaseInShopCart;

internal sealed class UpdatePurchaseInShopCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPatch("v1/shop-carts/update-purchase/{shopCartId:int}/{bikeId:int}", async (
                int shopCartId,
                int bikeId,
                UpdatePurchaseInShopCartRequest request,
                UpdatePurchaseInShopCartHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(shopCartId, bikeId, request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("ShopCart");
}
