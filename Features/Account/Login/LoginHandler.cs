using BikeClub.Data;
using BikeClub.Features.Account.Shared;
using BikeClub.SharedKernel;
using BikeClub.SharedKernel.Results;
using BikeClub.SharedKernel.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BikeClub.Features.Account.Login;

internal sealed class LoginHandler
{
    private readonly DataContext _db;
    private readonly IValidator<LoginRequest> _validator;
    private readonly ICryptographerService _cryptographer;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        DataContext db,
        IValidator<LoginRequest> validator,
        ICryptographerService cryptographer,
        ITokenService tokenService)
    {
        _db = db;
        _validator = validator;
        _cryptographer = cryptographer;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return ValidationResult<LoginResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var hashPassword = _cryptographer.Hash(request.Password);

        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Email == request.Email && u.Password == hashPassword)
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return AccountErrors.InvalidCredentials;

        user.Password = "***********";

        var token = _tokenService.GenerateToken(user);
        var expiresIn = DateTime.UtcNow.AddHours(Settings.TokenExpirationHours);

        return new LoginResponse(user, token, expiresIn);
    }
}
