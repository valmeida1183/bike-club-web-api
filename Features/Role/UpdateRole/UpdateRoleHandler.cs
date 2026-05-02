using BikeClub.Data;
using BikeClub.Features.Role.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoleEntity = BikeClub.Domain.Entities.Role;

namespace BikeClub.Features.Role.UpdateRole;

internal sealed class UpdateRoleHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdateRoleRequest> _validator;

    public UpdateRoleHandler(DataContext db, IValidator<UpdateRoleRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<RoleResponse>> Handle(string name, UpdateRoleRequest request, CancellationToken ct)
    {
        if (!name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            return RoleErrors.NameMismatch;

        var validation = await _validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return ValidationResult<RoleResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var exists = await _db.Roles.AnyAsync(r => r.Name == name, ct);
        if (!exists)
            return RoleErrors.NotFound;

        var role = new RoleEntity { Name = request.Name, Description = request.Description };

        _db.Entry(role).State = EntityState.Modified;
        await _db.SaveChangesAsync(ct);

        return RoleResponse.From(role);
    }
}
