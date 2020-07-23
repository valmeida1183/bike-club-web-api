using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase 
    {
        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model, 
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception ex)
            {                
                return ExceptionHandlerService.HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody] Category model, 
            [FromServices] DataContext context)
        {
            if (id != model.Id)
            {
                return BadRequest("Cannot change Id of Category");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception ex)
            {                
               return ExceptionHandlerService.HandleException(ex);
            }
        }

        [HttpDelete("id:int")]
        public async Task<ActionResult<Category>> Delete(
            int id, 
            [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." }); 
            }

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new { message = "Category removed with success." });
            }
            catch (System.Exception ex)
            {                
               return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}