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
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Invalid Email or Password" });
            }

            var hashPassword = CryptographerService.Hash(model.Password);

            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.Email == model.Email && u.Password == hashPassword)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Invalid Email or Password" });
            }

            //Esconde a senha
            user.Password = "***********";

            var token = TokenService.GenerateToken(user);
            var expiresIn = DateTime.UtcNow.AddHours(Settings.TokenExpirationHours);

            return Ok(new { user = user, token = token, expiresIn = expiresIn });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = context.Users.Count(u => u.Email == model.Email);

            if (user > 0)
            {
                return BadRequest(new { message = "This email exists already" });
            }

            try
            {
                // Força o user a ser um ciclista
                model.RoleName = RoleStatic.Cyclist;
                model.Password = CryptographerService.Hash(model.Password);

                // Cria o carrinho de compras vazio para o usuário
                var shopCart = new ShopCart
                {
                    UserId = model.Id,
                    PurchaseDate = DateTimeOffset.UtcNow,
                    TotalAmount = 0
                };

                model.ShopCart = shopCart;

                context.Users.Add(model);
                await context.SaveChangesAsync();

                //Esconde a senha
                model.Password = "***********";

                var token = TokenService.GenerateToken(model);
                var expiresIn = DateTime.UtcNow.AddHours(Settings.TokenExpirationHours);

                return Ok(new { user = model, token = token, expiresIn = expiresIn });
            }
            catch (System.Exception ex)
            {
                return ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}