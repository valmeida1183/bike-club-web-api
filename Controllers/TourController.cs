using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using BikeClub.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/tours")]
    [Authorize]
    public class TourController : ControllerBase 
    {
        private readonly DataContext context;

        public TourController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tour>>> Get()
        {
            var tours = await context.Tours.AsNoTracking().ToListAsync();
            return Ok(tours);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tour>> GetById(int id)
        {
            var tour = await context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            return Ok(tour);
        }

        [HttpPost]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Tour>> Post([FromBody] Tour model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Tours.Add(model);
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
        public async Task<ActionResult<Tour>> Put(int id, [FromBody] Tour model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "Cannot change Id of Tour."});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Tour>(model).State = EntityState.Modified;
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
        public async Task<ActionResult<Tour>> Delete(int id)
        {
            var tour = await context.Tours.FirstOrDefaultAsync(t => t.Id == id);
            if (tour == null)
            {
                return NotFound(new { message = "Tour not found." });
            }

            try
            {
                context.Tours.Remove(tour);
                await context.SaveChangesAsync();

                return Ok(new { message = "Tour removed with success." });
            }
            catch (System.Exception ex)
            {
                return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}