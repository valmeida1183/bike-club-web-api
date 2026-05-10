using BikeClub.Data;
using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.ShopCart.UpdatePurchaseInShopCart;

internal sealed class UpdatePurchaseInShopCartHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdatePurchaseInShopCartRequest> _validator;
    private readonly ITotalAmountCalculator _calculator;

    public UpdatePurchaseInShopCartHandler(
        DataContext db,
        IValidator<UpdatePurchaseInShopCartRequest> validator,
        ITotalAmountCalculator calculator)
    {
        _db = db;
        _validator = validator;
        _calculator = calculator;
    }

    public async Task<Result<ShopCartResponse>> Handle(
        int shopCartId, int bikeId, UpdatePurchaseInShopCartRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<ShopCartResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var purchase = await _db.Purchases
            .FirstOrDefaultAsync(p => p.ShopCartId == shopCartId && p.BikeId == bikeId, ct);

        if (purchase is null)
            return Result.Failure<ShopCartResponse>(ShopCartErrors.PurchaseNotFound);

        purchase.Quantity = request.Quantity;
        await _db.SaveChangesAsync(ct);

        var shopCart = await _db.ShopCarts
            .Include(sc => sc.Address)
            .Include(sc => sc.Purchases)
            .ThenInclude(p => p.Bike)
            .FirstOrDefaultAsync(sc => sc.Id == shopCartId, ct);

        if (shopCart is null)
            return Result.Failure<ShopCartResponse>(ShopCartErrors.ShopCartNotFound);

        shopCart.TotalAmount = _calculator.Calculate(shopCart.Purchases);
        await _db.SaveChangesAsync(ct);

        return ShopCartResponse.From(shopCart);
    }
}
