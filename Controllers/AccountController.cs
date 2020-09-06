using System.Linq;
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
    //[ApiController] // utilizando uma controller com a notation "ApiController" -> https://docs.microsoft.com/pt-br/aspnet/core/web-api/?view=aspnetcore-3.1
    [Route("v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext context;

        public AccountController(DataContext context)
        {
            this.context = context;
        }        

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate ([FromBody] User model) 
        {             
            var hashPassword = CryptographerSerivce.Hash(model.Password);           

            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.Email == model.Email && u.Password == hashPassword)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Invalid Email or Password"});
            }

            var token = TokenService.GenerateToken(user);
            //Esconde a senha
            user.Password = "***********";

            return Ok(new {user = user, token = token});
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] User model) 
        {            
            if (!ModelState.IsValid)
            {
               return BadRequest(ModelState);
            }

            try
            {
                // Força o user a ser um ciclista
                model.RoleName = RoleStatic.Cyclist;
                model.Password = CryptographerSerivce.Hash(model.Password);              

                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "***********";

                return Ok(model);
            }
            catch (System.Exception ex)
            {                
                return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}