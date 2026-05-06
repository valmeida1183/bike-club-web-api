using BikeClub.Data;
using BikeClub.Features.Tour.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using TourEntity = BikeClub.Domain.Entities.Tour;

namespace BikeClub.Features.Tour.CreateTour;

internal sealed class CreateTourHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateTourRequest> _validator;

    public CreateTourHandler(DataContext db, IValidator<CreateTourRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<TourResponse>> Handle(CreateTourRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<TourResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var tour = new TourEntity
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description,
            MonitorId = request.MonitorId,
            DifficultyId = request.DifficultyId,
            AddressId = request.AddressId
        };

        _db.Tours.Add(tour);
        await _db.SaveChangesAsync(ct);

        return TourResponse.From(tour);
    }
}
