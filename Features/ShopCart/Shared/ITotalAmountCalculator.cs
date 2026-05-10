using PurchaseEntity = BikeClub.Domain.Entities.Purchase;

namespace BikeClub.Features.ShopCart.Shared;

public interface ITotalAmountCalculator
{
    decimal Calculate(IEnumerable<PurchaseEntity> purchases);
}
