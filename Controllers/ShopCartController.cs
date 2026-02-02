using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers;

[Route("v1/shop-carts")]
[Authorize]
public class ShopCartController : ControllerBase
{
  private readonly DataContext context;

  public ShopCartController(DataContext context)
  {
    this.context = context;
  }

  [HttpGet("{userId:int}")]
  public async Task<ActionResult<ShopCart>> GetByUserId(int userId)
  {
    var shopCart = await context.ShopCarts
        .AsNoTracking()
        .Include(sc => sc.Purchases)
        .ThenInclude(p => p.Bike)
        .FirstOrDefaultAsync(sc => sc.UserId == userId);

    return Ok(shopCart);
  }

  [HttpPost]
  public async Task<ActionResult<ShopCart>> CreateShopCart([FromBody] ShopCart model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    try
    {
      context.ShopCarts.Add(model);
      await context.SaveChangesAsync();

      return Ok(model);
    }
    catch (Exception ex)
    {
      return ExceptionHandlerService.HandleException(ex);
    }
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult<ShopCart>> UpdateShopCart(int id, [FromBody] ShopCart model)
  {
    if (id != model.Id)
    {
      return BadRequest(new { message = "Cannot change Id of ShopCart." });
    }

    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var existingShopCart = await context.ShopCarts
      .Include(sc => sc.Purchases)
      .ThenInclude(p => p.Bike)
      .FirstOrDefaultAsync(sc => sc.Id == id);

    if (existingShopCart == null)
    {
      return NotFound();
    }

    try
    {
      UpdatePurchaseList(existingShopCart, model.Purchases);
      existingShopCart.TotalAmount = await CalculateTotalAmount(existingShopCart.Purchases);

      await context.SaveChangesAsync();

      return Ok(existingShopCart);
    }
    catch (Exception ex)
    {
      return ExceptionHandlerService.HandleException(ex);
    }
  }

  private void UpdatePurchaseList(ShopCart existingShopCart, ICollection<Purchase> updatedPurchases)
  {
    var purchasesToRemove = existingShopCart.Purchases
      .Where(p => !updatedPurchases.Any(mp => mp.BikeId == p.BikeId && mp.ShopCartId == p.ShopCartId))
      .ToList();

    foreach (var purchase in purchasesToRemove)
    {
      context.Purchases.Remove(purchase);
    }

    existingShopCart.Purchases.Clear();
    foreach (var updatedPurchase in updatedPurchases)
    {
      var purchase = new Purchase
      {
        BikeId = updatedPurchase.BikeId,
        ShopCartId = updatedPurchase.ShopCartId,
        Quantity = updatedPurchase.Quantity
      };

      existingShopCart.Purchases.Add(purchase);
    }
  }

  private async Task<decimal> CalculateTotalAmount(IEnumerable<Purchase> purchases)
  {
    decimal total = 0m;
    var bikeIds = purchases.Select(p => p.BikeId).ToList();
    var bikes = await context.Bikes
      .Where(b => bikeIds.Contains(b.Id))
      .ToDictionaryAsync(b => b.Id);

    foreach (var purchase in purchases)
    {
      if (bikes.TryGetValue(purchase.BikeId, out var bike))
      {
        total += bike.Price * purchase.Quantity;
      }
    }

    return total;
  }
}