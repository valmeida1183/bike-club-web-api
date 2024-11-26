using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using BikeClub.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/genders")]
    [Authorize]
    public class GenderController : ControllerBase
    {
        private readonly DataContext context;

        public GenderController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Gender>>> Get()
        {
            var genders = await context.Genders.AsNoTracking().ToListAsync();
            return Ok(genders);
        }

        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<Gender>> GetByCode(string code)
        {
            var gender = await context.Genders.AsNoTracking().FirstOrDefaultAsync(g => g.Code == code);
            return Ok(gender);
        }

        [HttpPost]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<Gender>> Post([FromBody] Gender model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Genders.Add(model);
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