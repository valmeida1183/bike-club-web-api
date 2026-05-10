using FluentValidation;

namespace BikeClub.Features.Purchase.CreatePurchase;

public class CreatePurchaseValidator : AbstractValidator<CreatePurchaseRequest>
{
    public CreatePurchaseValidator()
    {
        RuleFor(x => x.ShopCartId).GreaterThan(0);
        RuleFor(x => x.BikeId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
    }
}
