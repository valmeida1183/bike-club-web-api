namespace BikeClub.Features.ShopCart.AddPurchaseToShopCart;

public record AddPurchaseToShopCartRequest(int ShopCartId, int BikeId, int Quantity);
