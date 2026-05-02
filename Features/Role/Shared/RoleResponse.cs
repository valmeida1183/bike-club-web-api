using RoleEntity = BikeClub.Domain.Entities.Role;

namespace BikeClub.Features.Role.Shared;

public record RoleResponse(string? Name, string? Description)
{
    public static RoleResponse From(RoleEntity role) =>
        new(role.Name, role.Description);
}
