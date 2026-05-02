using BikeClub.Domain.Entities;

namespace BikeClub.Features.Account.Login;

public record LoginResponse(User User, string Token, DateTime ExpiresIn);
