using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/roles")]
    public class RoleController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Role>>> Get([FromServices] DataContext context)
        {
            var roles = await context.Roles.AsNoTracking().ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Role>> GetByName(
            string name, 
            [FromServices] DataContext context)
        {
           var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name);
           return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> Post (
            [FromBody] Role model, 
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Roles.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {                
                return BadRequest(new { message = "Cannot create a role." });
            }            
        }

        [HttpPut("{name}")]
        public async Task<ActionResult<Role>> Put (
            string name, 
            [FromBody] Role model, 
            [FromServices] DataContext context)
        {
            if (!name.Equals(model.Name, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cannot change name of Role");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
               // Desta Forma o EF vai verificar o que foi mudado em relação ao registro do banco e persistir no banco somente o que foi mudado.
                context.Entry<Role>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model); 
            }
            catch (DbUpdateConcurrencyException)
            {                
                return BadRequest(new { message = "Role is already updated." });
            }
            catch
            {
                return BadRequest(new { message = "Cannot update a role." });
            }
        }
    }
}