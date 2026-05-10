using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.ShopCart.GetShopCartByUserId;

internal sealed class GetShopCartByUserIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/shop-carts/{userId:int}", async (
                int userId,
                GetShopCartByUserIdHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(userId, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("ShopCart");
}
