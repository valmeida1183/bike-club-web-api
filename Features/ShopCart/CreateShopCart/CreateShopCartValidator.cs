using FluentValidation;

namespace BikeClub.Features.ShopCart.CreateShopCart;

public class CreateShopCartValidator : AbstractValidator<CreateShopCartRequest>
{
    public CreateShopCartValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.PurchaseDate).NotEmpty();
    }
}
