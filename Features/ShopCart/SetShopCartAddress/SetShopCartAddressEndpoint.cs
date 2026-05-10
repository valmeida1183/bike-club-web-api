using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.ShopCart.SetShopCartAddress;

internal sealed class SetShopCartAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPatch("v1/shop-carts/{shopCartId:int}/address", async (
                int shopCartId,
                SetShopCartAddressRequest request,
                SetShopCartAddressHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(shopCartId, request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("ShopCart");
}
