using BikeClub.Data;
using BikeClub.Features.Role.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Role.GetRole;

internal sealed class GetRoleHandler
{
    private readonly DataContext _db;

    public GetRoleHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(CancellationToken ct)
    {
        var roles = await _db.Roles
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<RoleResponse>>(
            roles.Select(RoleResponse.From).ToList());
    }
}
