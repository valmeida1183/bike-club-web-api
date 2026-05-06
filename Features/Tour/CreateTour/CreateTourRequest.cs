using BikeClub.Features.Tour.Shared;

namespace BikeClub.Features.Tour.CreateTour;

public record CreateTourRequest(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string? Description,
    int MonitorId,
    int DifficultyId,
    int AddressId) : ITourRequest;
