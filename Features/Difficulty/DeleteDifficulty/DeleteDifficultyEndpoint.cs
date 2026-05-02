using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Difficulty.DeleteDifficulty;

internal sealed class DeleteDifficultyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("v1/difficulties/{id:int}", async (
                int id,
                DeleteDifficultyHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Difficulty");
}
