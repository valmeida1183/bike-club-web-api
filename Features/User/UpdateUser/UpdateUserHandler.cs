using BikeClub.Data;
using BikeClub.Features.User.Shared;
using UserEntity = BikeClub.Domain.Entities.User;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.User.UpdateUser;

internal sealed class UpdateUserHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdateUserRequest> _validator;

    public UpdateUserHandler(DataContext db, IValidator<UpdateUserRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<UserResponse>> Handle(int id, UpdateUserRequest request, CancellationToken ct)
    {
        if (id != request.Id)
            return UserErrors.IdMismatch;

        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<UserResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var user = new UserEntity
        {
            Id = request.Id,
            Email = request.Email,
            Password = request.Password,
            Phone = request.Phone,
            Name = request.Name,
            LastName = request.LastName,
            GenderCode = request.GenderCode,
            RoleName = request.RoleName
        };

        _db.Entry<UserEntity>(user).State = EntityState.Modified;
        await _db.SaveChangesAsync(ct);

        return UserResponse.From(user);
    }
}
