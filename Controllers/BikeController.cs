using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using BikeClub.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/bikes")]
    [Authorize]
    public class BikeController : ControllerBase
    {
        private readonly DataContext context;

        public BikeController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet()]
        public async Task<ActionResult<List<Bike>>> Get([FromQuery] int? categoryId = null, string? genderCode = null,
        decimal? price = null, int? gears = null, decimal? frameSize = null, decimal? rimSize = null)
        {
            var bikes = await context.Bikes
                .AsNoTracking()
                .Where(b => (!categoryId.HasValue || b.CategoryId == categoryId) &&
                        (string.IsNullOrEmpty(genderCode) || b.GenderCode == genderCode) &&
                        (!price.HasValue || b.Price <= price) &&
                        (!gears.HasValue || b.Gears <= gears) &&
                        (!frameSize.HasValue || b.FrameSize <= frameSize) &&
                        (!rimSize.HasValue || b.RimSize <= rimSize))
                .Include(b => b.Gender)
                .Include(b => b.Category)
                .ToListAsync();

            return Ok(bikes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Bike>> GetById(int id)
        {
            var bike = await context.Bikes.AsNoTracking()
            .Include(b => b.Category)
            .Include(b => b.Gender)
            .FirstOrDefaultAsync(b => b.Id == id);

            return Ok(bike);
        }

        [HttpPost]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Bike>> Post([FromBody] Bike model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Bikes.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception ex)
            {
                return ExceptionHandlerService.HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Bike>> Put(int id, [FromBody] Bike model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "Cannot change Id of Bike." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Bike>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception ex)
            {
                return ExceptionHandlerService.HandleException(ex);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Bike>> Delete(int id)
        {
            var bike = await context.Bikes.FirstOrDefaultAsync(b => b.Id == id);
            if (bike == null)
            {
                return NotFound(new { message = "Bike not found." });
            }

            try
            {
                context.Bikes.Remove(bike);
                await context.SaveChangesAsync();

                return Ok(new { message = "Bike removed with success." });
            }
            catch (System.Exception ex)
            {
                return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}