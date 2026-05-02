using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Difficulty.GetDifficultyById;

internal sealed class GetDifficultyByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/difficulties/{id:int}", async (
                int id,
                GetDifficultyByIdHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
        .AllowAnonymous()
        .WithTags("Difficulty");
}
