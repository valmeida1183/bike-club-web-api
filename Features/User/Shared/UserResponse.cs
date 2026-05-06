using UserEntity = BikeClub.Domain.Entities.User;

namespace BikeClub.Features.User.Shared;

public record UserResponse(
    int Id,
    string Email,
    string Password,
    string Phone,
    string Name,
    string LastName,
    string GenderCode,
    string? RoleName)
{
    public static UserResponse From(UserEntity user) =>
        new(user.Id, user.Email, user.Password, user.Phone, user.Name, user.LastName, user.GenderCode, user.RoleName);
}
