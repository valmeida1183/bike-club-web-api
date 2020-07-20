using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/difficulties")]
    public class DifficultyController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Difficulty>>> Get([FromServices] DataContext context)
        {
            var difficulties = await context.Difficulties.AsNoTracking().ToListAsync();
            return Ok(difficulties);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Difficulty>> GetById(int id, [FromServices] DataContext context)
        {
            var difficulty = await context.Difficulties.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            return Ok(difficulty);
        }

        [HttpPost]
        public async Task<ActionResult<Difficulty>> Post(
            [FromBody] Difficulty model, 
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            try
            {
                if (await context.Difficulties.AnyAsync(d => d.Id == model.Id))
                {
                    return Conflict(new { message = $"Difficulty with Id: {model.Id} already exists." });
                }

                context.Difficulties.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {                
                return BadRequest(new { message = "Cannot create a difficulty." });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Difficulty>> Put(
            int id, 
            [FromBody] Difficulty model, 
            [FromServices] DataContext context)
        {
           if (id != model.Id)
            {
                return BadRequest("Cannot change Id of Difficulty");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
               // Desta Forma o EF vai verificar o que foi mudado em relação ao registro do banco e persistir no banco somente o que foi mudado.
                context.Entry<Difficulty>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model); 
            }
            catch (DbUpdateConcurrencyException)
            {                
                return BadRequest(new { message = "Difficulty is already updated." });
            }
            catch
            {
                return BadRequest(new { message = "Cannot update a Difficulty." });
            } 
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Difficulty>> Delete (
            int id, 
            [FromServices] DataContext context)
        {
            var difficulty = await context.Difficulties.FirstOrDefaultAsync(x => x.Id == id);
            if (difficulty == null)
            {
                return NotFound(new { message = "Difficulty not found." });
            }

            try
            {
                context.Difficulties.Remove(difficulty);
                await context.SaveChangesAsync();
                return Ok(new { message = "Difficulty removed with success." });
            }
            catch
            {
                return BadRequest(new { message = "Cannot remove difficulty." });
            }
        }
    }
}