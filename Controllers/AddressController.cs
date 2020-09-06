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
    [Route("v1/addresses")]
    [Authorize]
    public class AddressController: ControllerBase
    {
        private readonly DataContext context;

        public AddressController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]        
        public async Task<ActionResult<List<Address>>> Get()
        {
            var addresses = await context.Addresses.AsNoTracking().ToListAsync();
            return Ok(addresses);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Address>> GetById(int id)
        {
            var address = await context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<ActionResult<Address>> Post([FromBody] Address model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Addresses.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (System.Exception ex)
            {
                return ExceptionHandlerService.HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Address>> Put(int id, [FromBody] Address model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "Cannot change Id of Address."});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Address>(model).State = EntityState.Modified;
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
        public async Task<ActionResult<Address>> Delete(int id)
        {
            var address = await context.Addresses.FirstOrDefaultAsync(a => a.Id == id);
            if (address == null)
            {
                return NotFound(new { message = "Address not found."});
            }

            try
            {
                context.Addresses.Remove(address);
                await context.SaveChangesAsync();

                return Ok(new { message = "Address removed with success." });

            }
            catch (System.Exception ex)
            {                
                return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}