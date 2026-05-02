using BikeClub.Features.Difficulty.Shared;

namespace BikeClub.Features.Difficulty.CreateDifficulty;

public record CreateDifficultyRequest(string? Name) : IDifficultyRequest;
