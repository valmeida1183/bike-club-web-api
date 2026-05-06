using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Http;
using BikeClub.SharedKernel.Static;
using Microsoft.AspNetCore.Authorization;

namespace BikeClub.Features.Tour.UpdateTour;

internal sealed class UpdateTourEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("v1/tours/{id:int}", async (
                int id,
                UpdateTourRequest request,
                UpdateTourHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, request, ct)).ToIResult())
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })
        .WithTags("Tour");
}
