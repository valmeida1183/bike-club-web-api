using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Difficulty.UpdateDifficulty;

internal sealed class UpdateDifficultyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("v1/difficulties/{id:int}", async (
                int id,
                UpdateDifficultyRequest request,
                UpdateDifficultyHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Difficulty");
}
