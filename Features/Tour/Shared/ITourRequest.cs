namespace BikeClub.Features.Tour.Shared;

public interface ITourRequest
{
    DateTimeOffset StartDate { get; }
    DateTimeOffset EndDate { get; }
    string? Description { get; }
    int MonitorId { get; }
    int DifficultyId { get; }
    int AddressId { get; }
}
