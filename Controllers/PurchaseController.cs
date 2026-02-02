using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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