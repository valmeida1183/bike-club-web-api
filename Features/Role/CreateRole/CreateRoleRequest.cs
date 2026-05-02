using BikeClub.Features.Role.Shared;

namespace BikeClub.Features.Role.CreateRole;

public record CreateRoleRequest(string? Name, string? Description) : IRoleRequest;
