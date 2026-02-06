using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BikeClub.Controllers;

[Route("v1/purchases")]
[Authorize]
public class PurchaseController : ControllerBase
{
  private readonly DataContext context;

  public PurchaseController(DataContext context)
  {
    this.context = context;
  }

  [HttpPost]
  public async Task<ActionResult<Purchase>> Save([FromBody] Purchase purchase)
  {

    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var currentPurchase = await context.Purchases
        .AsNoTracking()
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

      return Ok(currentPurchase ?? purchase); // calulate and return the total amount of shopping cart
    }
    catch (Exception ex)
    {
      return ExceptionHandlerService.HandleException(ex);
    }
  }

  [HttpDelete("{id:int}")]
  public async Task<ActionResult<Purchase>> Delete(int id)
  {
    var purchase = await context.Purchases.FindAsync(id);
    if (purchase == null)
    {
      return NotFound();
    }

    try
    {
      context.Purchases.Remove(purchase);
      await context.SaveChangesAsync();

      return Ok(new { message = "Purchase removed with success." });
    }
    catch (Exception ex)
    {
      return ExceptionHandlerService.HandleException(ex);
    }
  }
}