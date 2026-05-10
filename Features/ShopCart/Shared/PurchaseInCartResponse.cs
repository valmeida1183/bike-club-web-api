using PurchaseEntity = BikeClub.Domain.Entities.Purchase;

namespace BikeClub.Features.ShopCart.Shared;

public record PurchaseInCartResponse(int ShopCartId, int BikeId, int Quantity, BikeInCartResponse? Bike)
{
    public static PurchaseInCartResponse From(PurchaseEntity purchase) =>
        new(purchase.ShopCartId, purchase.BikeId, purchase.Quantity,
            purchase.Bike is not null ? BikeInCartResponse.From(purchase.Bike) : null);
}
