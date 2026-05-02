using BikeClub.Data;
using BikeClub.Features.Role.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using RoleEntity = BikeClub.Domain.Entities.Role;

namespace BikeClub.Features.Role.CreateRole;

internal sealed class CreateRoleHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateRoleRequest> _validator;

    public CreateRoleHandler(DataContext db, IValidator<CreateRoleRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<RoleResponse>> Handle(CreateRoleRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<RoleResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var role = new RoleEntity { Name = request.Name, Description = request.Description };

        _db.Roles.Add(role);
        await _db.SaveChangesAsync(ct);

        return RoleResponse.From(role);
    }
}
