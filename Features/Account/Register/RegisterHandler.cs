using BikeClub.Data;
using BikeClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BikeClub.Features.Account.Shared;
using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Results;
using BikeClub.SharedKernel.Services;
using BikeClub.SharedKernel.Static;
using FluentValidation;

namespace BikeClub.Features.Account.Register;

internal sealed class RegisterHandler
{
    private readonly DataContext _db;
    private readonly IValidator<RegisterRequest> _validator;
    private readonly ICryptographerService _cryptographer;
    private readonly ITokenService _tokenService;

    public RegisterHandler(
        DataContext db,
        IValidator<RegisterRequest> validator,
        ICryptographerService cryptographer,
        ITokenService tokenService)
    {
        _db = db;
        _validator = validator;
        _cryptographer = cryptographer;
        _tokenService = tokenService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<RegisterResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (emailExists)
            return AccountErrors.EmailAlreadyExists;

        var user = new User
        {
            Email = request.Email,
            Password = _cryptographer.Hash(request.Password),
            Phone = request.Phone,
            Name = request.Name,
            LastName = request.LastName,
            GenderCode = request.GenderCode,
            RoleName = RoleStatic.Cyclist
        };

        var shopCart = new ShopCart
        {
            UserId = user.Id,
            PurchaseDate = DateTimeOffset.UtcNow,
            TotalAmount = 0
        };

        user.ShopCart = shopCart;

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        _db.Entry(user).State = EntityState.Detached;
        var token = _tokenService.GenerateToken(user);
        var expiresIn = DateTime.UtcNow.AddHours(Settings.TokenExpirationHours);
        user.Password = "***********";

        return new RegisterResponse(user, token, expiresIn);
    }
}
