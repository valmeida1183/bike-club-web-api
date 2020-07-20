using System.Collections.Generic;
using System.Threading.Tasks;
using BikeClub.Data;
using BikeClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/genders")]
    public class GenderController : ControllerBase  
    {
        [HttpGet]
        public async Task<ActionResult<List<Gender>>> Get ([FromServices] DataContext context)
        {
            var genders = await context.Genders.AsNoTracking().ToListAsync();
            return Ok(genders);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Gender>> GetByCode (
            string code, 
            [FromServices] DataContext context)
        {
            var gender = await context.Genders.AsNoTracking().FirstOrDefaultAsync(g => g.Code == code);
            return Ok(gender);
        }
    }
}