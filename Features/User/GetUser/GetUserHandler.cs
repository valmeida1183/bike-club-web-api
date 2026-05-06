using BikeClub.Data;
using BikeClub.Features.User.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.User.GetUser;

internal sealed class GetUserHandler
{
    private readonly DataContext _db;

    public GetUserHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(CancellationToken ct)
    {
        var users = await _db.Users
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<UserResponse>>(
            users.Select(UserResponse.From).ToList());
    }
}
