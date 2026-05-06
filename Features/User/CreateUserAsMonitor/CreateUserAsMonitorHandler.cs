using BikeClub.Data;
using BikeClub.Features.User.Shared;
using UserEntity = BikeClub.Domain.Entities.User;
using BikeClub.SharedKernel.Results;
using BikeClub.SharedKernel.Services;
using BikeClub.SharedKernel.Static;
using FluentValidation;

namespace BikeClub.Features.User.CreateUserAsMonitor;

internal sealed class CreateUserAsMonitorHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateUserAsMonitorRequest> _validator;
    private readonly ICryptographerService _cryptographer;

    public CreateUserAsMonitorHandler(
        DataContext db,
        IValidator<CreateUserAsMonitorRequest> validator,
        ICryptographerService cryptographer)
    {
        _db = db;
        _validator = validator;
        _cryptographer = cryptographer;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserAsMonitorRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<UserResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var user = new UserEntity
        {
            Email = request.Email,
            Password = _cryptographer.Hash(request.Password),
            Phone = request.Phone,
            Name = request.Name,
            LastName = request.LastName,
            GenderCode = request.GenderCode,
            RoleName = RoleStatic.Monitor
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return UserResponse.From(user);
    }
}
