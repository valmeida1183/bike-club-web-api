using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.ShopCart.CreateShopCart;

internal sealed class CreateShopCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/shop-carts", async (
                CreateShopCartRequest request,
                CreateShopCartHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("ShopCart");
}
