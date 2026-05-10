using BikeClub.Features.Address.Shared;
using ShopCartEntity = BikeClub.Domain.Entities.ShopCart;

namespace BikeClub.Features.ShopCart.Shared;

public record ShopCartResponse(
    int Id,
    DateTimeOffset PurchaseDate,
    decimal TotalAmount,
    int UserId,
    int? AddressId,
    AddressResponse? Address,
    IReadOnlyList<PurchaseInCartResponse> Purchases)
{
    public static ShopCartResponse From(ShopCartEntity sc) =>
        new(sc.Id,
            sc.PurchaseDate,
            sc.TotalAmount,
            sc.UserId,
            sc.AddressId,
            sc.Address is not null ? AddressResponse.From(sc.Address) : null,
            sc.Purchases.Select(PurchaseInCartResponse.From).ToList());
}
