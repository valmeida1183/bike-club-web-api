using BikeClub.Features.Difficulty.Shared;

namespace BikeClub.Features.Difficulty.UpdateDifficulty;

public record UpdateDifficultyRequest(int Id, string? Name) : IDifficultyRequest;
