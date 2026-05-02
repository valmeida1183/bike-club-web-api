using BikeClub.Data;
using BikeClub.Features.Role.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Role.GetRoleByName;

internal sealed class GetRoleByNameHandler
{
    private readonly DataContext _db;

    public GetRoleByNameHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<RoleResponse>> Handle(string name, CancellationToken ct)
    {
        var role = await _db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == name, ct);

        if (role is null)
            return RoleErrors.NotFound;

        return RoleResponse.From(role);
    }
}
