using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Account.Register;

internal sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/accounts/register", async (
                RegisterRequest request,
                RegisterHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
        .AllowAnonymous()
        .WithTags("Account");
}
