using FluentValidation;

namespace BikeClub.Features.ShopCart.UpdatePurchaseInShopCart;

public class UpdatePurchaseInShopCartValidator : AbstractValidator<UpdatePurchaseInShopCartRequest>
{
    public UpdatePurchaseInShopCartValidator()
    {
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
    }
}
