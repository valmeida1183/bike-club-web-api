using BikeClub.Data;
using BikeClub.Features.User.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.User.GetUserById;

internal sealed class GetUserByIdHandler
{
    private readonly DataContext _db;

    public GetUserByIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<UserResponse>> Handle(int id, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null)
            return UserErrors.NotFound;

        return UserResponse.From(user);
    }
}
