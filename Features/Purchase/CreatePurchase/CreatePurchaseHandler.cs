using BikeClub.Data;
using BikeClub.Features.Purchase.Shared;
using BikeClub.SharedKernel.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PurchaseEntity = BikeClub.Domain.Entities.Purchase;

namespace BikeClub.Features.Purchase.CreatePurchase;

internal sealed class CreatePurchaseHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreatePurchaseRequest> _validator;

    public CreatePurchaseHandler(DataContext db, IValidator<CreatePurchaseRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<PurchaseResponse>> Handle(CreatePurchaseRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<PurchaseResponse>.WithErrors(
                validation.Errors
                    .Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation))
                    .ToArray());

        var existing = await _db.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ShopCartId == request.ShopCartId && p.BikeId == request.BikeId, ct);

        if (existing is not null)
        {
            existing.Quantity += request.Quantity;
            _db.Purchases.Update(existing);
            await _db.SaveChangesAsync(ct);
            return PurchaseResponse.From(existing);
        }

        var purchase = new PurchaseEntity
        {
            ShopCartId = request.ShopCartId,
            BikeId = request.BikeId,
            Quantity = request.Quantity
        };

        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync(ct);

        return PurchaseResponse.From(purchase);
    }
}
