using BikeClub.Features.User.Shared;

namespace BikeClub.Features.User.CreateUserAsMonitor;

public record CreateUserAsMonitorRequest(
    string Email,
    string Password,
    string Phone,
    string Name,
    string LastName,
    string GenderCode) : IUserPersonalInfoRequest;
