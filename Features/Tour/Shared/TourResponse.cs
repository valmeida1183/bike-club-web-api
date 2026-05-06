using TourEntity = BikeClub.Domain.Entities.Tour;

namespace BikeClub.Features.Tour.Shared;

public record TourResponse(
    int Id,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string? Description,
    int MonitorId,
    int DifficultyId,
    int AddressId)
{
    public static TourResponse From(TourEntity tour) =>
        new(tour.Id, tour.StartDate, tour.EndDate, tour.Description,
            tour.MonitorId, tour.DifficultyId, tour.AddressId);
}
