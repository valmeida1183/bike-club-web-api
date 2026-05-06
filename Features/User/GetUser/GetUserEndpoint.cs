using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.User.GetUser;

internal sealed class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/users", async (
                GetUserHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(ct)).ToIResult())
        .RequireAuthorization()
        .WithTags("User");
}
