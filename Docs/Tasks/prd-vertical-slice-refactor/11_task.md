# Task 11.0: Purchase Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `PurchaseController`. One operation migrated: `CreatePurchase` (POST — with upsert-on-existing semantics). Note the unusual upsert behavior on POST: if a Purchase already exists for the same `(ShopCartId, BikeId)`, the handler increments its `Quantity` instead of inserting a duplicate. Preserve that exactly.

> **Scope amendment (decided during implementation):** `DeletePurchase` (`DELETE /v1/purchases/{id:int}`) was **not migrated and the route was dropped**. Reason: `Purchase` has a composite PK `(BikeId, ShopCartId)` with no single `int Id` field. The original `FindAsync(id)` with one int on a two-part composite key throws `InvalidOperationException` at runtime — the endpoint never worked. The ShopCart slice already covers purchase removal via `DELETE /v1/shop-carts/remove-purchase/{shopCartId}/{bikeId}`. Team decision: drop the broken route rather than invent a new composite-key route under the Purchase prefix.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**, **`minimal-api`**.
</skills>

<requirements>
- `Features/Purchase/CreatePurchase/` — `POST v1/purchases`, `RequireAuthorization()`. Returns `Result<PurchaseResponse>`.
  - Validator mirrors `Purchase` entity annotations.
  - Handler: look up existing `Purchase` by `(ShopCartId, BikeId)` with `AsNoTracking().FirstOrDefaultAsync`.
    - If found: increment `Quantity += request.Quantity`, `context.Purchases.Update(existing)`, save, return `Result.Success(updatedPurchase)`.
    - If not: `context.Purchases.Add(new)`, save, return `Result.Success(newPurchase)`.
  - Success **value** matches today's `currentPurchase ?? purchase` body, now nested at `response.value` of the `Result<PurchaseResponse>` envelope.
- ~~`Features/Purchase/DeletePurchase/`~~ — **dropped** (see scope amendment above; route was non-functional in the original).
- Delete `Controllers/PurchaseController.cs`.
</requirements>

## Subtasks

- [x] 11.1 Validator.
- [x] 11.2 `CreatePurchase` (upsert logic).
- [x] ~~11.3 `DeletePurchase`~~ — dropped (scope amendment).
- [x] 11.4 Delete `Controllers/PurchaseController.cs`.
- [ ] 11.5 Manual Verification.

## Implementation Details

See `techspec.md` → "API Endpoints" (purchases row). The upsert logic uses `AsNoTracking()` on the lookup then `Update()` — verify this still works with EF change-tracking after migrating (EF `Update` re-attaches the entity as Modified, which is the intended behavior).

## Success Criteria

- `POST /v1/purchases` migrated; `Controllers/PurchaseController.cs` gone.
- Upsert semantics preserved: repeated POSTs with identical `(ShopCartId, BikeId)` increment quantity, not insert.
- `DELETE /v1/purchases/{id}` intentionally dropped (see scope amendment).

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] `POST /v1/purchases` with `{ shopCartId: X, bikeId: Y, quantity: 2 }` → 200 with `Result<PurchaseResponse>` envelope; `response.value` is the new purchase.
  - [ ] Same POST again → 200, `response.value.quantity` is 4 (incremented, not duplicated).
  - [ ] Verify in DB: only one row for `(X, Y)` exists after both calls.
  - [ ] `POST /v1/purchases` with invalid body (e.g., missing bikeId) → 400 with validation Result envelope (`errors[]` populated).
  - ~~`DELETE /v1/purchases/{existingId}`~~ — dropped (scope amendment).
  - ~~`DELETE /v1/purchases/9999`~~ — dropped (scope amendment).

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Purchase/*` (new)
- `Controllers/PurchaseController.cs` (deleted)
