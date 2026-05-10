namespace BikeClub.Features.Purchase.CreatePurchase;

public record CreatePurchaseRequest(int ShopCartId, int BikeId, int Quantity);
