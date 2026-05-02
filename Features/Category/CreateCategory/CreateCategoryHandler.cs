using BikeClub.Data;
using BikeClub.Features.Category.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using CategoryEntity = BikeClub.Domain.Entities.Category;

namespace BikeClub.Features.Category.CreateCategory;

internal sealed class CreateCategoryHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateCategoryRequest> _validator;

    public CreateCategoryHandler(DataContext db, IValidator<CreateCategoryRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<CategoryResponse>> Handle(CreateCategoryRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<CategoryResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var category = new CategoryEntity { Name = request.Name };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);

        return CategoryResponse.From(category);
    }
}
