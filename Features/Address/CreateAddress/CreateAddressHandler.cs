using BikeClub.Data;
using BikeClub.Features.Address.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using AddressEntity = BikeClub.Domain.Entities.Address;

namespace BikeClub.Features.Address.CreateAddress;

internal sealed class CreateAddressHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateAddressRequest> _validator;

    public CreateAddressHandler(DataContext db, IValidator<CreateAddressRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<AddressResponse>> Handle(CreateAddressRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<AddressResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var address = new AddressEntity
        {
            Street = request.Street,
            Complement = request.Complement,
            State = request.State,
            City = request.City,
            ZipCode = request.ZipCode
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(ct);

        return AddressResponse.From(address);
    }
}
