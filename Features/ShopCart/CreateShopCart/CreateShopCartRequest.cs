namespace BikeClub.Features.ShopCart.CreateShopCart;

public record CreateShopCartRequest(int UserId, DateTimeOffset PurchaseDate);
