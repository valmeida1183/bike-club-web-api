using BikeClub.Data;
using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.ShopCart.GetShopCartByUserId;

internal sealed class GetShopCartByUserIdHandler
{
    private readonly DataContext _db;

    public GetShopCartByUserIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<ShopCartResponse?>> Handle(int userId, CancellationToken ct)
    {
        var shopCart = await _db.ShopCarts
            .AsNoTracking()
            .Include(sc => sc.Address)
            .Include(sc => sc.Purchases)
            .ThenInclude(p => p.Bike)
            .FirstOrDefaultAsync(sc => sc.UserId == userId, ct);

        return Result.Success<ShopCartResponse?>(
            shopCart is not null ? ShopCartResponse.From(shopCart) : null);
    }
}
