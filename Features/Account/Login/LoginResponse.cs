using UserEntity = BikeClub.Domain.Entities.User;

namespace BikeClub.Features.Account.Login;

public record LoginResponse(UserEntity User, string Token, DateTime ExpiresIn);
