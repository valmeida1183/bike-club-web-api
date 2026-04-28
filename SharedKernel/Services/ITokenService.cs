using BikeClub.Domain.Entities;

namespace BikeClub.SharedKernel.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
