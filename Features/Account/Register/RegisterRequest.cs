namespace BikeClub.Features.Account.Register;

public record RegisterRequest(
    string Email,
    string Password,
    string Phone,
    string Name,
    string LastName,
    string GenderCode);
