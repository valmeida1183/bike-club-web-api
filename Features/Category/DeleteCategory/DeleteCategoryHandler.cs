using BikeClub.Data;
using BikeClub.Features.Category.Shared;
using BikeClub.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Category.DeleteCategory;

internal sealed class DeleteCategoryHandler
{
    private readonly DataContext _db;

    public DeleteCategoryHandler(DataContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(int id, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
