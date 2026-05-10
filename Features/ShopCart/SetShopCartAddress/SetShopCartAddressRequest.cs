namespace BikeClub.Features.ShopCart.SetShopCartAddress;

public record SetShopCartAddressRequest(
    string? Street,
    string? Complement,
    string? State,
    string? City,
    int ZipCode);
