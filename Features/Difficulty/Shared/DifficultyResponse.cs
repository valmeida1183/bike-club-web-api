using DifficultyEntity = BikeClub.Domain.Entities.Difficulty;

namespace BikeClub.Features.Difficulty.Shared;

public record DifficultyResponse(int Id, string? Name)
{
    public static DifficultyResponse From(DifficultyEntity difficulty) =>
        new(difficulty.Id, difficulty.Name);
}
