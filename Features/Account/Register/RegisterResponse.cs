using BikeClub.Domain.Entities;

namespace BikeClub.Features.Account.Register;

public record RegisterResponse(User User, string Token, DateTime ExpiresIn);
