using BikeClub.Data;
using BikeClub.Features.Category.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Category.GetCategory;

internal sealed class GetCategoryHandler
{
    private readonly DataContext _db;

    public GetCategoryHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(CancellationToken ct)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<CategoryResponse>>(
            categories.Select(CategoryResponse.From).ToList());
    }
}
