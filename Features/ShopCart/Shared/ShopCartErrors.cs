using BikeClub.SharedKernel.Results;

namespace BikeClub.Features.ShopCart.Shared;

public static class ShopCartErrors
{
    public static readonly Error PurchaseNotFound = new(
        "ShopCart.PurchaseNotFound",
        "The purchase with the specified bike was not found in the cart",
        ErrorType.NotFound);

    public static readonly Error ShopCartNotFound = new(
        "ShopCart.ShopCartNotFound",
        "The shop cart with the specified ID was not found",
        ErrorType.NotFound);
}
