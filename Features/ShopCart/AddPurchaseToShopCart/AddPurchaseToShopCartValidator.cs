using FluentValidation;

namespace BikeClub.Features.ShopCart.AddPurchaseToShopCart;

public class AddPurchaseToShopCartValidator : AbstractValidator<AddPurchaseToShopCartRequest>
{
    public AddPurchaseToShopCartValidator()
    {
        RuleFor(x => x.ShopCartId).GreaterThan(0);
        RuleFor(x => x.BikeId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
    }
}
