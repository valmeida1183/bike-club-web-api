namespace BikeClub.Features.User.Shared;

public interface IUserPersonalInfoRequest
{
    string Email { get; }
    string Password { get; }
    string Phone { get; }
    string Name { get; }
    string LastName { get; }
}
