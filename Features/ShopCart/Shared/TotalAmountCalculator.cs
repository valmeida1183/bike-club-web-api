using PurchaseEntity = BikeClub.Domain.Entities.Purchase;

namespace BikeClub.Features.ShopCart.Shared;

internal sealed class TotalAmountCalculator : ITotalAmountCalculator
{
    public decimal Calculate(IEnumerable<PurchaseEntity> purchases)
    {
        decimal total = 0m;
        foreach (var purchase in purchases)
            total += purchase.Bike!.Price * purchase.Quantity;
        return total;
    }
}
