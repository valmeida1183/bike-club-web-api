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
  public async Task<ActionResult<ShopCart>> Post([FromBody] ShopCart model)
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
}