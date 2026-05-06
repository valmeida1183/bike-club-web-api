using BikeClub.Features.User.Shared;

namespace BikeClub.Features.User.UpdateUser;

public record UpdateUserRequest(
    int Id,
    string Email,
    string Password,
    string Phone,
    string Name,
    string LastName,
    string GenderCode,
    string? RoleName) : IUserPersonalInfoRequest;
