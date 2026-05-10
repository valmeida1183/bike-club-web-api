using PurchaseEntity = BikeClub.Domain.Entities.Purchase;

namespace BikeClub.Features.Purchase.Shared;

public record PurchaseResponse(int ShopCartId, int BikeId, int Quantity)
{
    public static PurchaseResponse From(PurchaseEntity purchase) =>
        new(purchase.ShopCartId, purchase.BikeId, purchase.Quantity);
}
