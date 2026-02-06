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

  [HttpPost("add-purchase")]
  public async Task<ActionResult<ShopCart>> AddPurchaseToShopCart([FromBody] Purchase purchase)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var currentPurchase = await context.Purchases
      .FirstOrDefaultAsync(p => p.ShopCartId == purchase.ShopCartId && p.BikeId == purchase.BikeId);

    if (currentPurchase != null)
    {
      currentPurchase.Quantity += purchase.Quantity;
      context.Purchases.Update(currentPurchase);
    }
    else
    {
      context.Purchases.Add(purchase);
    }

    try
    {
      await context.SaveChangesAsync();

      var shopCart = await context.ShopCarts
      .Include(sc => sc.Purchases)
      .ThenInclude(p => p.Bike)
      .FirstOrDefaultAsync(sc => sc.Id == purchase.ShopCartId);

      if (shopCart == null)
      {
        return BadRequest(new { message = "ShopCart not found." });
      }

      shopCart.TotalAmount = CalculateTotalAmount(shopCart.Purchases);

      await context.SaveChangesAsync();

      return Ok(shopCart);
    }
    catch (Exception ex)
    {
      return ExceptionHandlerService.HandleException(ex);
    }
  }

  [HttpDelete("remove-purchase/{shopCartId:int}/{bikeId:int}")]
  public async Task<ActionResult> RemovePurchaseFromShopCart(int shopCartId, int bikeId)
  {
    var purchase = await context.Purchases
      .FirstOrDefaultAsync(p => p.ShopCartId == shopCartId && p.BikeId == bikeId);

    if (purchase == null)
    {
      return NotFound(new { message = "Purchase not found." });
    }

    context.Purchases.Remove(purchase);

    try
    {
      await context.SaveChangesAsync();

      var shopCart = await context.ShopCarts
        .Include(sc => sc.Purchases)
        .ThenInclude(p => p.Bike)
        .FirstOrDefaultAsync(sc => sc.Id == shopCartId);

      if (shopCart == null)
      {
        return BadRequest(new { message = "ShopCart not found." });
      }

      shopCart.TotalAmount = CalculateTotalAmount(shopCart.Purchases);

      await context.SaveChangesAsync();

      return Ok(shopCart);
    }
    catch (Exception ex)
    {
      return ExceptionHandlerService.HandleException(ex);
    }
  }

  private decimal CalculateTotalAmount(IEnumerable<Purchase> purchases)
  {
    decimal total = 0m;

    foreach (var purchase in purchases)
    {
      total += purchase.Bike!.Price * purchase.Quantity;
    }

    return total;
  }
}