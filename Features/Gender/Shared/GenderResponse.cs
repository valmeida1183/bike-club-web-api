using GenderEntity = BikeClub.Domain.Entities.Gender;

namespace BikeClub.Features.Gender.Shared;

public record GenderResponse(string? Code, string? Description)
{
    public static GenderResponse From(GenderEntity gender) =>
        new(gender.Code, gender.Description);
}
