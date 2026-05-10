using BikeClub.Data;
using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.ShopCart.RemovePurchaseFromShopCart;

internal sealed class RemovePurchaseFromShopCartHandler
{
    private readonly DataContext _db;
    private readonly ITotalAmountCalculator _calculator;

    public RemovePurchaseFromShopCartHandler(DataContext db, ITotalAmountCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<Result<ShopCartResponse>> Handle(int shopCartId, int bikeId, CancellationToken ct)
    {
        var purchase = await _db.Purchases
            .FirstOrDefaultAsync(p => p.ShopCartId == shopCartId && p.BikeId == bikeId, ct);

        if (purchase is null)
            return Result.Failure<ShopCartResponse>(ShopCartErrors.PurchaseNotFound);

        _db.Purchases.Remove(purchase);
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
