using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Difficulty.CreateDifficulty;

internal sealed class CreateDifficultyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/difficulties", async (
                CreateDifficultyRequest request,
                CreateDifficultyHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Difficulty");
}
