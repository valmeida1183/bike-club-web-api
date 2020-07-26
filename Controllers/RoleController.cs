using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/roles")]
    public class RoleController : ControllerBase
    {
        private readonly DataContext context;

        public RoleController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Role>>> Get()
        {
            var roles = await context.Roles.AsNoTracking().ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Role>> GetByName(
            string name)
        {
           var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name);
           return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> Post (
            [FromBody] Role model)
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
            catch (System.Exception ex)
            {                
               return ExceptionHandlerService.HandleException(ex);
            }       
        }

        [HttpPut("{name}")]
        public async Task<ActionResult<Role>> Put (
            string name, 
            [FromBody] Role model)
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
            catch (System.Exception ex)
            {                
               return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}