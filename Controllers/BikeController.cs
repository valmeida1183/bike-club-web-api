using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/bikes")]
    public class BikeController : ControllerBase
    {
        private readonly DataContext context;

        public BikeController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Bike>>> Get()
        {
            var bikes = await context.Bikes.AsNoTracking().ToListAsync();
            return Ok(bikes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Bike>> GetById(int id)
        {
            var bike = await context.Bikes.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return Ok(bike);
        }

        [HttpPost]
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
        public async Task<ActionResult<Bike>> Put(int id, [FromBody] Bike model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "Cannot change Id of Bike."});
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
        public async Task<ActionResult<Bike>> Delete(int id)
        {
            var bike = await context.Bikes.FirstOrDefaultAsync(b => b.Id == id);
            if (bike == null)
            {
                return NotFound(new { message = "Bike not found."});
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