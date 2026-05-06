using BikeClub.Data;
using BikeClub.Features.Tour.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TourEntity = BikeClub.Domain.Entities.Tour;

namespace BikeClub.Features.Tour.UpdateTour;

internal sealed class UpdateTourHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdateTourRequest> _validator;

    public UpdateTourHandler(DataContext db, IValidator<UpdateTourRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<TourResponse>> Handle(int id, UpdateTourRequest request, CancellationToken ct)
    {
        if (id != request.Id)
            return TourErrors.IdMismatch;

        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<TourResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var tour = new TourEntity
        {
            Id = request.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description,
            MonitorId = request.MonitorId,
            DifficultyId = request.DifficultyId,
            AddressId = request.AddressId
        };

        _db.Entry(tour).State = EntityState.Modified;
        await _db.SaveChangesAsync(ct);

        return TourResponse.From(tour);
    }
}
