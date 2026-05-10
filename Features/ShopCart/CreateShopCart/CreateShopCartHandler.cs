using BikeClub.Data;
using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using ShopCartEntity = BikeClub.Domain.Entities.ShopCart;

namespace BikeClub.Features.ShopCart.CreateShopCart;

internal sealed class CreateShopCartHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateShopCartRequest> _validator;

    public CreateShopCartHandler(DataContext db, IValidator<CreateShopCartRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<ShopCartResponse>> Handle(CreateShopCartRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<ShopCartResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var shopCart = new ShopCartEntity
        {
            UserId = request.UserId,
            PurchaseDate = request.PurchaseDate,
            TotalAmount = 0m
        };

        _db.ShopCarts.Add(shopCart);
        await _db.SaveChangesAsync(ct);

        return ShopCartResponse.From(shopCart);
    }
}
