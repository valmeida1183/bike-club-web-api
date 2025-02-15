using BikeClub.Data;
using BikeClub.Models;
using BikeClub.Services;
using BikeClub.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Controllers
{
    [Route("v1/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext context;

        public UserController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = RoleStatic.Monitor)]
        public async Task<ActionResult<User>> PostMonitor([FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                model.RoleName = RoleStatic.Monitor;
                model.Password = CryptographerService.Hash(model.Password);

                context.Users.Add(model);
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
        public async Task<ActionResult<User>> Put(
            int id,
            [FromBody] User model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "Cannot change Id of User." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
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