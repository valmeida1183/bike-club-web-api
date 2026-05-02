using BikeClub.Data;
using BikeClub.Features.Category.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Category.GetCategoryById;

internal sealed class GetCategoryByIdHandler
{
    private readonly DataContext _db;

    public GetCategoryByIdHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result<CategoryResponse>> Handle(int id, CancellationToken ct)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (category is null)
            return CategoryErrors.NotFound;

        return CategoryResponse.From(category);
    }
}
