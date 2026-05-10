using BikeClub.Data;
using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PurchaseEntity = BikeClub.Domain.Entities.Purchase;

namespace BikeClub.Features.ShopCart.AddPurchaseToShopCart;

internal sealed class AddPurchaseToShopCartHandler
{
    private readonly DataContext _db;
    private readonly IValidator<AddPurchaseToShopCartRequest> _validator;
    private readonly ITotalAmountCalculator _calculator;

    public AddPurchaseToShopCartHandler(
        DataContext db,
        IValidator<AddPurchaseToShopCartRequest> validator,
        ITotalAmountCalculator calculator)
    {
        _db = db;
        _validator = validator;
        _calculator = calculator;
    }

    public async Task<Result<ShopCartResponse>> Handle(AddPurchaseToShopCartRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<ShopCartResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var currentPurchase = await _db.Purchases
            .FirstOrDefaultAsync(p => p.ShopCartId == request.ShopCartId && p.BikeId == request.BikeId, ct);

        if (currentPurchase is not null)
        {
            currentPurchase.Quantity += request.Quantity;
            _db.Purchases.Update(currentPurchase);
        }
        else
        {
            _db.Purchases.Add(new PurchaseEntity
            {
                ShopCartId = request.ShopCartId,
                BikeId = request.BikeId,
                Quantity = request.Quantity
            });
        }

        await _db.SaveChangesAsync(ct);

        var shopCart = await _db.ShopCarts
            .Include(sc => sc.Purchases)
            .ThenInclude(p => p.Bike)
            .FirstOrDefaultAsync(sc => sc.Id == request.ShopCartId, ct);

        if (shopCart is null)
            return Result.Failure<ShopCartResponse>(ShopCartErrors.ShopCartNotFound);

        shopCart.TotalAmount = _calculator.Calculate(shopCart.Purchases);
        await _db.SaveChangesAsync(ct);

        return ShopCartResponse.From(shopCart);
    }
}
