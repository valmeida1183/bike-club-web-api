using BikeClub.Data;
using BikeClub.Features.Bike.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using BikeEntity = BikeClub.Domain.Entities.Bike;

namespace BikeClub.Features.Bike.CreateBike;

internal sealed class CreateBikeHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateBikeRequest> _validator;

    public CreateBikeHandler(DataContext db, IValidator<CreateBikeRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<BikeResponse>> Handle(CreateBikeRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<BikeResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var bike = new BikeEntity
        {
            Gears = request.Gears,
            FrameSize = request.FrameSize,
            RimSize = request.RimSize,
            Model = request.Model,
            Description = request.Description,
            Price = request.Price,
            Image = request.Image,
            GenderCode = request.GenderCode,
            CategoryId = request.CategoryId
        };

        _db.Bikes.Add(bike);
        await _db.SaveChangesAsync(ct);

        return BikeResponse.From(bike);
    }
}
