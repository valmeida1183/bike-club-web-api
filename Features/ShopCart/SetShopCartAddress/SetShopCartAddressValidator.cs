using FluentValidation;

namespace BikeClub.Features.ShopCart.SetShopCartAddress;

public class SetShopCartAddressValidator : AbstractValidator<SetShopCartAddressRequest>
{
    public SetShopCartAddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(50).MinimumLength(3);
        RuleFor(x => x.Complement).NotEmpty().MaximumLength(50).MinimumLength(1);
        RuleFor(x => x.State).NotEmpty().Length(2);
        RuleFor(x => x.City).NotEmpty().MaximumLength(30).MinimumLength(1);
        RuleFor(x => x.ZipCode).GreaterThan(0);
    }
}
