using BikeClub.Data;
using BikeClub.Features.Category.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CategoryEntity = BikeClub.Domain.Entities.Category;

namespace BikeClub.Features.Category.UpdateCategory;

internal sealed class UpdateCategoryHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdateCategoryRequest> _validator;

    public UpdateCategoryHandler(DataContext db, IValidator<UpdateCategoryRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<CategoryResponse>> Handle(int id, UpdateCategoryRequest request, CancellationToken ct)
    {
        if (id != request.Id)
            return CategoryErrors.IdMismatch;

        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<CategoryResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var category = new CategoryEntity { Id = request.Id, Name = request.Name };

        _db.Entry(category).State = EntityState.Modified;
        await _db.SaveChangesAsync(ct);

        return CategoryResponse.From(category);
    }
}
