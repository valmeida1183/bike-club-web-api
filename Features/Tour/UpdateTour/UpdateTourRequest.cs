using BikeClub.Features.Tour.Shared;

namespace BikeClub.Features.Tour.UpdateTour;

public record UpdateTourRequest(
    int Id,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string? Description,
    int MonitorId,
    int DifficultyId,
    int AddressId) : ITourRequest;
