using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Account.Login;

internal sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/accounts/login", async (
                LoginRequest request,
                LoginHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .AllowAnonymous()
        .WithTags("Account");
}
