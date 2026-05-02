using BikeClub.Features.Role.Shared;

namespace BikeClub.Features.Role.UpdateRole;

public record UpdateRoleRequest(string? Name, string? Description) : IRoleRequest;
