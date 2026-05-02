using BikeClub.Data;
using BikeClub.Features.Address.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AddressEntity = BikeClub.Domain.Entities.Address;

namespace BikeClub.Features.Address.UpdateAddress;

internal sealed class UpdateAddressHandler
{
    private readonly DataContext _db;
    private readonly IValidator<UpdateAddressRequest> _validator;

    public UpdateAddressHandler(DataContext db, IValidator<UpdateAddressRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<AddressResponse>> Handle(int id, UpdateAddressRequest request, CancellationToken ct)
    {
        if (id != request.Id)
            return AddressErrors.IdMismatch;

        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<AddressResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var address = new AddressEntity
        {
            Id = request.Id,
            Street = request.Street,
            Complement = request.Complement,
            State = request.State,
            City = request.City,
            ZipCode = request.ZipCode
        };

        _db.Entry(address).State = EntityState.Modified;
        await _db.SaveChangesAsync(ct);

        return AddressResponse.From(address);
    }
}
