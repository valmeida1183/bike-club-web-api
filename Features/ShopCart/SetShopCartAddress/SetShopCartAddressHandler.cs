using BikeClub.Data;
using BikeClub.Features.ShopCart.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AddressEntity = BikeClub.Domain.Entities.Address;

namespace BikeClub.Features.ShopCart.SetShopCartAddress;

internal sealed class SetShopCartAddressHandler
{
    private readonly DataContext _db;
    private readonly IValidator<SetShopCartAddressRequest> _validator;

    public SetShopCartAddressHandler(DataContext db, IValidator<SetShopCartAddressRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<ShopCartResponse>> Handle(
        int shopCartId, SetShopCartAddressRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<ShopCartResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var shopCart = await _db.ShopCarts.FindAsync(new object?[] { shopCartId }, ct);

        if (shopCart is null)
            return Result.Failure<ShopCartResponse>(ShopCartErrors.ShopCartNotFound);

        var address = new AddressEntity
        {
            Street = request.Street,
            Complement = request.Complement,
            State = request.State,
            City = request.City,
            ZipCode = request.ZipCode
        };

        address.Id = shopCart.AddressId ?? 0;
        shopCart.Address = address;
        await _db.SaveChangesAsync(ct);

        var result = await _db.ShopCarts
            .AsNoTracking()
            .Include(sc => sc.Address)
            .Include(sc => sc.Purchases)
            .ThenInclude(p => p.Bike)
            .FirstOrDefaultAsync(sc => sc.Id == shopCartId, ct);

        if (result is null)
            return Result.Failure<ShopCartResponse>(ShopCartErrors.ShopCartNotFound);

        return ShopCartResponse.From(result);
    }
}
