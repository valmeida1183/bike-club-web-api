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
    [Route("v1/categories")]
    [Authorize]
    public class CategoryController : ControllerBase 
    {
        private readonly DataContext context;

        public CategoryController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ResponseCache(VaryByHeader="User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Get()
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model)
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
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody] Category model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "Cannot change Id of Category"} );
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Category>> Delete(int id)
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