using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BikeClub.Models;
using Microsoft.IdentityModel.Tokens;

namespace BikeClub.Services
{
    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Settings.Secret;

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Cannot generate token because settings is not configured");
            }

            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.RoleName)
                }),
                Expires = DateTime.UtcNow.AddHours(Settings.TokenExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}