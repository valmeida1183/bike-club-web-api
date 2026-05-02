using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;

namespace BikeClub.Features.Gender.GetGenderByCode;

internal sealed class GetGenderByCodeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("v1/genders/{code}", async (
                string code,
                GetGenderByCodeHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(code, ct)).ToIResult())
        .AllowAnonymous()
        .WithTags("Gender");
}
