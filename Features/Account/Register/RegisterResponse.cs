using UserEntity = BikeClub.Domain.Entities.User;

namespace BikeClub.Features.Account.Register;

public record RegisterResponse(UserEntity User, string Token, DateTime ExpiresIn);
